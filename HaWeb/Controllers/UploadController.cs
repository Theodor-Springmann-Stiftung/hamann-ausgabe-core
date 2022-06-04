namespace HaWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using Microsoft.FeatureManagement.Mvc;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using HaWeb.Filters;
using HaWeb.XMLParser;
using HaWeb.Models;
using HaWeb.FileHelpers;

public class UploadController : Controller {
    // DI
    private IHaDocumentWrappper _lib;
    private IReaderService _readerService;
    private readonly long _fileSizeLimit;
    private readonly string _targetFilePath;
    private readonly IXMLService _xmlService;

    // Options
    private static readonly string[] _permittedExtensions = { ".xml" };
    private static readonly FormOptions _defaultFormOptions = new FormOptions();


    public UploadController(IHaDocumentWrappper lib, IReaderService readerService, IXMLService xmlService, IConfiguration config) {
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
    [Route("Admin/Upload/{id?}")]
    [FeatureGate(Features.AdminService)]
    [GenerateAntiforgeryTokenCookie]
    public IActionResult Index(string? id) {
        if (id != null) {
            var root = _xmlService.GetRoot(id);
            if (root == null) return error404();

            var roots = _xmlService.GetRoots();
            if (roots == null) return error404();

            var usedFiles = _xmlService.GetUsed();
            var availableFiles = _xmlService.GetAvailableFiles(id);

            var model = new UploadViewModel(root.Type, id, roots, availableFiles, usedFiles);
            return View("../Admin/Upload/Index", model);
        }
        else {
            var roots = _xmlService.GetRoots();
            if (roots == null) return error404();

            var usedFiles = _xmlService.GetUsed();

            var model = new UploadViewModel("Upload", id, roots, null, usedFiles);
            return View("../Admin/Upload/Index", model);
        }
    }

    private IActionResult error404() {
        Response.StatusCode = 404;
        return Redirect("/Error404");
    }
}