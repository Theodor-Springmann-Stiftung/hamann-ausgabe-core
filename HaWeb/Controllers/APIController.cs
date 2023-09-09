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
using HaWeb.XMLTests;
using System.Text.Json.Serialization;
using System.Text.Json;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using Microsoft.FeatureManagement.Mvc;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http.Features;
using System.Text;

// Controlling all the API-Endpoints
[FeatureGate(Features.AdminService)]
public class APIController : Controller {

    // DI
    private IHaDocumentWrappper _lib;
    private readonly IXMLFileProvider _xmlProvider;

    // Options
    private static readonly FormOptions _defaultFormOptions = new FormOptions();


    public APIController(IHaDocumentWrappper lib, IXMLFileProvider xmlProvider) {
        _lib = lib;
        _xmlProvider = xmlProvider;
    }

    [HttpPost]
    [Route("API/SetInProduction")]
    [DisableFormValueModelBinding]
    [ValidateAntiForgeryToken]
    [FeatureGate(Features.LocalPublishService, Features.AdminService)]
    public async Task<IActionResult> SetInProduction() {
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
            ModelState.AddModelError("Error", "Kein Dateiname.");
            return BadRequest(ModelState);
        }

        var newFile =  hF.Where(x => x.Name == filename);
        if (newFile == null || !newFile.Any()) {
            ModelState.AddModelError("Error", "Versuch, auf eine unverfügbare Datei zuzugreifen.");
            return BadRequest(ModelState);
        }

        _ = _lib.SetLibrary(newFile.First(), null, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Created("/", newFile.First());
    }
}