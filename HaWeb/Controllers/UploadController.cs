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
using Microsoft.AspNetCore.Mvc.Rendering;

public class UploadController : Controller {
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


    public UploadController(IHaDocumentWrappper lib, IReaderService readerService, IXMLService xmlService, IXMLProvider xmlProvider, IConfiguration config) {
        _lib = lib;
        _readerService = readerService;
        _xmlService = xmlService;
        _xmlProvider = xmlProvider;
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
        var roots = _xmlService.GetRootsList();
        if (roots == null) return error404();

        var hF = _xmlProvider.GetHamannFiles();
        List<FileModel>? hamannFiles = null;
        if (hF != null)
            hamannFiles = hF
                .OrderByDescending(x => x.LastModified)
                .Select(x => new FileModel(x.Name, string.Empty, x.LastModified.LocalDateTime, false, x == _xmlProvider.GetInProduction()))
                .ToList();

        var uF = _xmlService.GetUsedDictionary();
        var pF = _xmlService.GetInProduction();

        Dictionary<string, List<FileModel>?>? usedFiles = null;
        if (uF != null) {
            usedFiles = new Dictionary<string, List<FileModel>?>();
            foreach (var kv in uF) {
                if (kv.Value == null) continue;
                usedFiles.Add(kv.Key, XMLFileHelpers.ToFileModel(kv.Value, pF, uF));
            }
        }

        Dictionary<string, List<FileModel>?>? productionFiles = null;
        if (pF != null) {
            productionFiles = new Dictionary<string, List<FileModel>?>();
            foreach (var kv in pF) {
                if (kv.Value == null) continue;
                productionFiles.Add(kv.Key, XMLFileHelpers.ToFileModel(kv.Value, pF, uF));
            }
        }

        if (id != null) {
            id = id.ToLower();

            var root = _xmlService.GetRoot(id);
            if (root == null) return error404();

            var model = new UploadViewModel(root.Type, id, roots, usedFiles);
            model.ProductionFiles = productionFiles;
            model.HamannFiles = hamannFiles;
            model.AvailableFiles = XMLFileHelpers.ToFileModel(_xmlProvider.GetFiles(id), pF, uF);

            return View("../Admin/Upload/Index", model);
        } else {
            var model = new UploadViewModel("Upload & Veröffentlichen", id, roots, usedFiles);
            model.ProductionFiles = productionFiles;
            model.HamannFiles = hamannFiles;

            return View("../Admin/Upload/Index", model);
        }
    }

    private IActionResult error404() {
        Response.StatusCode = 404;
        return Redirect("/Error404");
    }
}