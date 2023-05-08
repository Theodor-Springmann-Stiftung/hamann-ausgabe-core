namespace HaWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using HaWeb.Filters;
using HaWeb.FileHelpers;
using HaWeb.XMLParser;
using HaWeb.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using Microsoft.FeatureManagement.Mvc;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http.Features;
using System.Text;

// Controlling all the API-Endpoints
public class APIController : Controller {

    // DI
    private IHaDocumentWrappper _lib;
    private IReaderService _readerService;
    private readonly long _fileSizeLimit;
    private readonly string _targetFilePath;
    private readonly IXMLService _xmlService;
    private readonly IXMLProvider _xmlProvider;

    // Options
    private static readonly string[] _permittedExtensions = { ".xml" };
    private static readonly FormOptions _defaultFormOptions = new FormOptions();


    public APIController(IHaDocumentWrappper lib, IReaderService readerService, IXMLService xmlService, IXMLProvider xmlProvider, IConfiguration config) {
        _lib = lib;
        _xmlProvider = xmlProvider;
        _readerService = readerService;
        _xmlService = xmlService;
        _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            _targetFilePath = config.GetValue<string>("StoredFilePathWindows");
        } else {
            _targetFilePath = config.GetValue<string>("StoredFilePathLinux");
        }
    }

    [HttpGet]
    [Route("API/Syntaxcheck/{id}")]
    [DisableFormValueModelBinding]
    [ValidateAntiForgeryToken]
    [FeatureGate(Features.UploadService, Features.AdminService)]
    public IActionResult SyntaxCheck(string id) {
        return Ok();
    }

    //// UPLOAD ////
    [HttpPost]
    [Route("API/Upload")]
    [DisableFormValueModelBinding]
    [ValidateAntiForgeryToken]
    [FeatureGate(Features.UploadService, Features.AdminService)]
    public async Task<IActionResult> Upload() {
        List<XMLRootDocument>? docs = null;
        //// 1. Stage: Check Request format and request spec
        // Checks the Content-Type Field (must be multipart + Boundary)
        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType)) {
            ModelState.AddModelError("Error", $"Wrong / No Content Type on the Request");
            return BadRequest(ModelState);
        }

        // Divides the multipart document into it's sections and sets up a reader
        var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
        var reader = new MultipartReader(boundary, HttpContext.Request.Body);
        MultipartSection? section = null;
        try {
            section = await reader.ReadNextSectionAsync();
        } catch (Exception ex) {
            ModelState.AddModelError("Error", "The Request is bad: " + ex.Message);
            return BadRequest(ModelState);
        }

        while (section != null) {
            // Multipart document content disposition header read for a section:
            // Starts with boundary, contains field name, content-dispo, filename, content-type
            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

            if (contentDisposition != null && contentDisposition.Name == "__RequestVerificationToken") {
                try {
                    section = await reader.ReadNextSectionAsync();
                } catch (Exception ex) {
                    ModelState.AddModelError("Error", "The Request is bad: " + ex.Message);
                }
                continue;
            }

            if (hasContentDispositionHeader && contentDisposition != null) {
                // Checks if it is a section with content-disposition, name, filename
                if (!MultipartRequestHelper.HasFileContentDisposition(contentDisposition)) {
                    ModelState.AddModelError("Error", $"Wrong Content-Dispostion Headers in Multipart Document");
                    return BadRequest(ModelState);
                }

                //// 2. Stage: Check File. Sanity checks on the file on a byte level, extension checking, is it empty etc.
                var streamedFileContent = await XMLFileHelpers.ProcessStreamedFile(
                    section, contentDisposition, ModelState,
                    _permittedExtensions, _fileSizeLimit);
                if (!ModelState.IsValid || streamedFileContent == null)
                    return BadRequest(ModelState);

                //// 3. Stage: Valid XML checking using a simple XDocument.Load()
                var xdocument = await XDocumentFileHelper.ProcessStreamedFile(streamedFileContent, ModelState);
                if (!ModelState.IsValid || xdocument == null)
                    return UnprocessableEntity(ModelState);

                //// 4. Stage: Is it a Hamann-Document? What kind?
                var retdocs = _xmlService.ProbeFile(xdocument, ModelState);
                if (!ModelState.IsValid || retdocs == null || !retdocs.Any())
                    return UnprocessableEntity(ModelState);

                //// 5. Stage: Saving the File(s)
                foreach (var doc in retdocs) {
                    // Physical saving
                    await _xmlProvider.Save(doc, _targetFilePath, ModelState);
                    // Setting the new docuemnt as used
                    _xmlService.Use(doc);
                    // Unsetting all old docuemnts as ununsed
                    _xmlService.AutoUse(doc.Prefix);
                    if (!ModelState.IsValid) return StatusCode(500, ModelState);
                    if (docs == null) docs = new List<XMLRootDocument>();
                    docs.Add(doc);
                }
                xdocument = null;
                retdocs = null;
                streamedFileContent = null;
            }

            try {
                section = await reader.ReadNextSectionAsync();
            } catch (Exception ex) {
                ModelState.AddModelError("Error", "The Request is bad: " + ex.Message);
                return BadRequest(ModelState);
            }
        }

        // 6. Stage: Success! Returning Ok, and redirecting 
        JsonSerializerOptions options = new() {
            ReferenceHandler = ReferenceHandler.Preserve,
            Converters = {
                new IdentificationStringJSONConverter()
            }
        };

        string json = JsonSerializer.Serialize(docs);
        return Created(nameof(UploadController), json);
    }


     //// PUBLISH ////
    [HttpPost]
    [Route("API/LocalPublish")]
    [DisableFormValueModelBinding]
    [ValidateAntiForgeryToken]
    [FeatureGate(Features.LocalPublishService, Features.AdminService, Features.UploadService)]
    public async Task<IActionResult> LocalPublish() {
        var element = _xmlService.MergeUsedDocuments(ModelState);
        if (!ModelState.IsValid || element == null)
            return BadRequest(ModelState);
        var savedfile = await _xmlProvider.SaveHamannFile(element, _targetFilePath, ModelState);
        if (!ModelState.IsValid || savedfile == null) {
            if (savedfile != null)
                _xmlProvider.DeleteHamannFile(savedfile.Name);
            return BadRequest(ModelState);
        }
        _ = _lib.SetLibrary(savedfile.PhysicalPath, ModelState);
        if (!ModelState.IsValid) {
            _xmlProvider.DeleteHamannFile(savedfile.Name);
            return BadRequest(ModelState);
        }
        _xmlProvider.SetInProduction(savedfile);
        _xmlService.SetInProduction();
        return Created("/", _xmlProvider.GetHamannFiles());
    }

    [HttpPost]
    [Route("API/SetUsed/{id}")]
    [DisableFormValueModelBinding]
    [ValidateAntiForgeryToken]
    [FeatureGate(Features.UploadService, Features.AdminService)]
    public async Task<IActionResult> SetUsed(string id) {
        var f = _xmlProvider.GetFiles(id);
        if (f == null) {
            ModelState.AddModelError("Error", "Wrong Endpoint");
            return BadRequest(ModelState);
        }

        var files = f.GetFileList();
        if (files == null) {
            ModelState.AddModelError("Error", "Wrong Endpoint");
            return BadRequest(ModelState);
        }

        List<XMLRootDocument>? newUsed = null;

        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType)) {
            ModelState.AddModelError("Error", $"Wrong / No Content Type on the Request");
            return BadRequest(ModelState);
        }

        // Same as above, check Upload()
        var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
        var reader = new MultipartReader(boundary, HttpContext.Request.Body);
        MultipartSection? section = null;
        try {
            section = await reader.ReadNextSectionAsync();
        } catch (Exception ex) {
            ModelState.AddModelError("Error", "The Request is bad: " + ex.Message);
            return BadRequest(ModelState);
        }

        while (section != null) {
            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

            if (contentDisposition != null && contentDisposition.Name == "__RequestVerificationToken") {
                try {
                    section = await reader.ReadNextSectionAsync();
                } catch (Exception ex) {
                    ModelState.AddModelError("Error", "The Request is bad: " + ex.Message);
                }
                continue;
            }

            var filename = string.Empty;
            if (hasContentDispositionHeader && contentDisposition != null) {
                if (!MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition)) {
                    ModelState.AddModelError("Error", $"Wrong Content-Dispostion Headers in Multipart Document");
                    return BadRequest(ModelState);
                }

                filename = XMLFileHelpers.StreamToString(section.Body, ModelState);
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var isFile = files.Where(x => x.FileName == filename);
                if (isFile == null || !isFile.Any()) {
                    ModelState.AddModelError("Error", "Tried to add a file that does not exist.");
                    return BadRequest(ModelState);
                }

                if (newUsed == null) newUsed = new List<XMLRootDocument>();
                newUsed.Add(isFile.First());
            }

            try {
                section = await reader.ReadNextSectionAsync();
            } catch (Exception ex) {
                ModelState.AddModelError("Error", "The Request is bad: " + ex.Message);
                return BadRequest(ModelState);
            }
        }

        if (newUsed != null && newUsed.Any()) {
            _xmlService.UnUse(id);
            newUsed.ForEach(x => _xmlService.Use(x));
        }

        return Created("/", newUsed);
    }


    [HttpPost]
    [Route("API/SetUsedHamann")]
    [DisableFormValueModelBinding]
    [ValidateAntiForgeryToken]
    [FeatureGate(Features.UploadService, Features.AdminService)]
    public async Task<IActionResult> SetUsedHamann() {
        var hF = _xmlProvider.GetHamannFiles();
        if (hF == null) {
            ModelState.AddModelError("Error", "There are no Hamman.xml files available.");
            return BadRequest(ModelState);
        }

        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType)) {
            ModelState.AddModelError("Error", $"Wrong / No Content Type on the Request");
            return BadRequest(ModelState);
        }

        // Same as above, check Upload()
        string? filename = null;
        var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
        var reader = new MultipartReader(boundary, HttpContext.Request.Body);
        MultipartSection? section = null;
        try {
            section = await reader.ReadNextSectionAsync();
        } catch (Exception ex) {
            ModelState.AddModelError("Error", "The Request is bad: " + ex.Message);
            return BadRequest(ModelState);
        }

        while (section != null) {
            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

            if (contentDisposition != null && contentDisposition.Name == "__RequestVerificationToken") {
                try {
                    section = await reader.ReadNextSectionAsync();
                } catch (Exception ex) {
                    ModelState.AddModelError("Error", "The Request is bad: " + ex.Message);
                }
                continue;
            }

            if (hasContentDispositionHeader && contentDisposition != null) {
                if (!MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition)) {
                    ModelState.AddModelError("Error", $"Wrong Content-Dispostion Headers in Multipart Document");
                    return BadRequest(ModelState);
                }

                filename = XMLFileHelpers.StreamToString(section.Body, ModelState);
                if (!ModelState.IsValid) return BadRequest(ModelState);
            }

            try {
                section = await reader.ReadNextSectionAsync();
            } catch (Exception ex) {
                ModelState.AddModelError("Error", "The Request is bad: " + ex.Message);
                return BadRequest(ModelState);
            }
        }

        if (filename == null) {
            ModelState.AddModelError("Error", "No filename given");
            return BadRequest(ModelState);
        }

        var newFile =  hF.Where(x => x.Name == filename);
        if (newFile == null || !newFile.Any()) {
            ModelState.AddModelError("Error", "Trying to set a unavailable file.");
            return BadRequest(ModelState);
        }

        _ = _lib.SetLibrary(newFile.First().PhysicalPath, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _xmlProvider.SetInProduction(newFile.First());

        return Created("/", newFile.First());
    }


    [HttpPost]
    [Route("API/SetStartEndYear")]
    [ValidateAntiForgeryToken]
    [FeatureGate(Features.UploadService, Features.AdminService)]
    public async Task<IActionResult>? SetStartEndYear(StartEndYear startendyear) {
        if (startendyear.StartYear > startendyear.EndYear) return BadRequest();
        _lib.SetStartEndYear(startendyear.StartYear, startendyear.EndYear);
        return Created("/", "");;
    }
}