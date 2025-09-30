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

public class FileListForm {
    public string file { get; set; }
    public string __RequestVerificationToken { get; set; }
}

// Controlling all the API-Endpoints
[FeatureGate(Features.AdminService)]
[ApiController]
public class APIController : Controller {

    // DI
    private readonly IHaDocumentWrappper _lib;
    private readonly IXMLFileProvider _xmlProvider;
    private readonly IXMLInteractionService _xmlService;
    
    // Options
    private static readonly FormOptions _defaultFormOptions = new FormOptions();
    private static readonly SemaphoreSlim _syntaxCheckLock = new SemaphoreSlim(1, 1);


    public APIController(IHaDocumentWrappper lib, IXMLInteractionService xmlService, IXMLFileProvider xmlProvider) {
        _lib = lib;
        _xmlProvider = xmlProvider;
        _xmlService = xmlService;
    }


    // TODO: this is trash
    [HttpPost]
    [Route("API/SetInProduction")]
    [ValidateAntiForgeryToken]
    [FeatureGate(Features.LocalPublishService, Features.AdminService)]
    public async Task<IActionResult> SetInProduction([FromForm] FileListForm _form) {
        var hF = _xmlProvider.GetHamannFiles();
        if (hF == null) {
            ModelState.AddModelError("Error", "There are no Hamman.xml files available.");
            return BadRequest(ModelState);
        }

        if (_form == null || String.IsNullOrWhiteSpace(_form.file)) {
            ModelState.AddModelError("Error", "Kein Dateiname.");
            return BadRequest(ModelState);
        }

        var newFile =  hF.Where(x => x.Name == _form.file);
        if (newFile == null || !newFile.Any()) {
            ModelState.AddModelError("Error", "Versuch, auf eine unverfügbare Datei zuzugreifen.");
            return BadRequest(ModelState);
        }

        _ = _lib.SetLibrary(newFile.First(), null, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Created("/", newFile.First());
    }

    [HttpGet]
    [Route("API/SyntaxCheck")]
    // [ValidateAntiForgeryToken]
    [DisableFormValueModelBinding]
    [FeatureGate(Features.SyntaxCheck, Features.AdminService)]
    public async Task<ActionResult<Dictionary<string, SyntaxCheckModel>?>> GetSyntaxCheck(string? id) {
        var SCCache = _xmlService.GetSCCache();
        if (SCCache == null) {
            await _syntaxCheckLock.WaitAsync();
            try {
                // Double-check after acquiring lock
                SCCache = _xmlService.GetSCCache();
                if (SCCache == null) {
                    var commit = _xmlProvider.GetGitState();
                    SCCache = _xmlService.Test(_xmlService.GetState(), commit != null ? commit.Commit : string.Empty);
                    _xmlService.SetSCCache(SCCache);
                }
            } finally {
                _syntaxCheckLock.Release();
            }
        }
        return Ok(SCCache);
    }
}