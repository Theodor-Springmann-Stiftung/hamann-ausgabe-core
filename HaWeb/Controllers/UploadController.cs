namespace HaWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using Microsoft.FeatureManagement.Mvc;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using HaWeb.Filters;
using HaWeb.FileHelpers;

public class UploadController : Controller
{
    // DI
    private ILibrary _lib;
    private IReaderService _readerService;
    private readonly long _fileSizeLimit;
    private readonly string _targetFilePath;

    // Options
    private static readonly string[] _permittedExtensions = { ".xml" };
    private static readonly FormOptions _defaultFormOptions = new FormOptions();


    public UploadController(ILibrary lib, IReaderService readerService, IConfiguration config)
    {
        _lib = lib;
        _readerService = readerService;
        _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        _targetFilePath = config.GetValue<string>("StoredFilesPath");
    }

    [HttpGet]
    [Route("Admin/Upload")]
    [FeatureGate(Features.UploadService)]
    [GenerateAntiforgeryTokenCookie]
    public IActionResult Index()
    {
        return View("../Admin/Upload/Index");
    }

    [HttpPost]
    [Route("Admin/Upload")]
    [DisableFormValueModelBinding]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Post() {

//// 1. Stage: Check Request and File on a byte-level
        // Checks the COntent-Type Field (must be multipart + Boundary)
        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        {
            ModelState.AddModelError("File", $"Wrong / No Content Type on the Request");
            return BadRequest(ModelState);
        }

        // Divides the multipart document into it's sections and sets up a reader
        var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
        var reader = new MultipartReader(boundary, HttpContext.Request.Body);
        var section = await reader.ReadNextSectionAsync();

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
                    ModelState.AddModelError("File", $"Wrong Content-Dispostion Headers in Multipart Document");
                    return BadRequest(ModelState);
                }

                // Sanity checks on the file on a byte level, extension checking, is it empty etc.
                var streamedFileContent = await XMLFileHelpers.ProcessStreamedFile(
                    section, contentDisposition, ModelState, 
                    _permittedExtensions, _fileSizeLimit);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

//// 2. Stage: Valid XML checking

//// 3. Stage: Is it a Hamann-Document? What kind?

//// 4. Stage: Get Filename for the stageing area

//// 5. Stage: Saving the File
                // // Encode Filename for display
                // var trustedFileNameForDisplay = WebUtility.HtmlEncode(contentDisposition.FileName.Value);


                // // TODO: generatre storage filename
                // var trustedFileNameForFileStorage = Path.GetRandomFileName();
                // using (var targetStream = System.IO.File.Create(Path.Combine(_targetFilePath, trustedFileNameForFileStorage)))
                //     await targetStream.WriteAsync(streamedFileContent);
            }

            // Drain any remaining section body that hasn't been consumed and
            // read the headers for the next section.
            section = await reader.ReadNextSectionAsync();
        }

//// Success! Return Last Created File View
        return Created(nameof(UploadController), null);
    }
}