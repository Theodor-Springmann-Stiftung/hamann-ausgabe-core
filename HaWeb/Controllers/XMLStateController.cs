namespace HaWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using HaWeb.Filters;
using HaWeb.XMLParser;
using HaWeb.Models;
using HaWeb.FileHelpers;
using HaWeb.BackgroundTask;

[FeatureGate(Features.AdminService)]
public class XMLStateController : Controller {
    // DI
    private IHaDocumentWrappper _lib;
    private readonly IXMLInteractionService _xmlService;
    private readonly IXMLFileProvider _xmlProvider;
    private readonly IMonitorLoop _loop;  
    public XMLStateController(IMonitorLoop loop, IHaDocumentWrappper lib, IXMLInteractionService xmlService, IXMLFileProvider xmlProvider) {
        _lib = lib;
        _xmlService = xmlService;
        _xmlProvider = xmlProvider;
        _loop = loop;
    }

    [HttpGet]
    [Route("Admin/XMLState/")]
    [FeatureGate(Features.AdminService)]
    [GenerateAntiforgeryTokenCookie]
    public IActionResult Index() {
        _loop.StartMonitorLoop();
        var library = _lib.GetLibrary();
        var roots = _xmlService.GetRootsList();
        if (roots == null) return error404();

        var hF = _xmlProvider.GetHamannFiles()?.OrderByDescending(x => x.LastModified).ToList();
        var mF = _xmlService.GetManagedFiles();
        var gD = _xmlProvider.GetGitData();
        var activeF = _lib.GetActiveFile();
        var vS = _xmlService.GetValidState();

        var model = new XMLStateViewModel("Dateiübersicht", gD, roots, hF, mF, vS) {
            ActiveFile = activeF,
        };
        return View("~/Views/Admin/Dynamic/XMLState.cshtml", model);
    }

    private IActionResult error404() {
        Response.StatusCode = 404;
        return Redirect("/Error404");
    }
}