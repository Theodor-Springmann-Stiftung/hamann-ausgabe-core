namespace HaWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using Microsoft.FeatureManagement.Mvc;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using HaWeb.Filters;
using HaWeb.FileHelpers;
using HaWeb.XMLParser;
using HaWeb.Models;
using System.Xml.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;

public class UploadController : Controller
{
    // DI
    private ILibrary _lib;
    private IReaderService _readerService;
    private readonly long _fileSizeLimit;
    private readonly string _targetFilePath;
    private readonly IXMLService _xmlService;

    // Options
    private static readonly string[] _permittedExtensions = { ".xml" };
    private static readonly FormOptions _defaultFormOptions = new FormOptions();


    public UploadController(ILibrary lib, IReaderService readerService, IXMLService xmlService, IConfiguration config)
    {
        _lib = lib;
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
    [Route("Admin/Upload")]
    [FeatureGate(Features.UploadService)]
    [GenerateAntiforgeryTokenCookie]
    public IActionResult Index()
    {
        var model = new UploadViewModel();
        model.AvailableRoots = _xmlService.GetRoots().Select(x => (x.Type, "")).ToList();
        return View("../Admin/Upload/Index", model);
    }


//// UPLOAD ////
    [HttpPost]
    [Route("Admin/Upload")]
    [DisableFormValueModelBinding]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Post() {
//// 1. Stage: Check Request format and request spec
        // Checks the Content-Type Field (must be multipart + Boundary)
        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        {
            ModelState.AddModelError("Error", $"Wrong / No Content Type on the Request");
            return BadRequest(ModelState);
        }

        // Divides the multipart document into it's sections and sets up a reader
        var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
        var reader = new MultipartReader(boundary, HttpContext.Request.Body);
        MultipartSection? section = null;
        try {
            section = await reader.ReadNextSectionAsync();
        }
        catch (Exception ex) {
            ModelState.AddModelError("Error", "The Request is bad: " + ex.Message);
        }

        while (section != null)
        {
            // Multipart document content disposition header read for a section:
            // Starts with boundary, contains field name, content-dispo, filename, content-type
            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
            if (hasContentDispositionHeader && contentDisposition != null)
            {
                // Checks if it is a section with content-disposition, name, filename
                if (!MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                {
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
                var docs = _xmlService.ProbeHamannFile(xdocument, ModelState);
                if (!ModelState.IsValid || docs == null || !docs.Any())
                    return UnprocessableEntity(ModelState);
                
//// 5. Stage: Saving the File(s)
                foreach (var doc in docs) {
                    var type = doc.Prefix;
                    var directory = Path.Combine(_targetFilePath, type);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    var path = Path.Combine(directory, doc.FileName);
                    try {
                        using (var targetStream = System.IO.File.Create(path))
                            await doc.Save(targetStream, ModelState);
                            if (!ModelState.IsValid) return StatusCode(500, ModelState);
                    }
                    catch (Exception ex) {
                        ModelState.AddModelError("Error",  "Speichern der Datei fehlgeschlagen: " + ex.Message);
                        return StatusCode(500, ModelState);
                    }
                }

// 6. State: Returning Ok, and redirecting 
                JsonSerializerOptions options = new() {
                    ReferenceHandler = ReferenceHandler.Preserve
                };

                string json = JsonSerializer.Serialize(docs);
                return Created(nameof(UploadController), json);
            }

           try
            {
                section = await reader.ReadNextSectionAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "The Request is bad: " + ex.Message);
            }
        }

//// Success! Return Last Created File View
        return Created(nameof(UploadController), null);
    }
}