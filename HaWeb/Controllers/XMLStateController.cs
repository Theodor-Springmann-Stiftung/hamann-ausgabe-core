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
    public XMLStateController(IHaDocumentWrappper lib, IXMLInteractionService xmlService, IXMLFileProvider xmlProvider) {
        _lib = lib;
        _xmlService = xmlService;
        _xmlProvider = xmlProvider;
    }

    [HttpGet]
    [Route("Admin/XMLState/")]
    [FeatureGate(Features.AdminService)]
    [GenerateAntiforgeryTokenCookie]
    public IActionResult Index() {
        var library = _lib.GetLibrary();
        var hF = _xmlProvider.GetHamannFiles()?.OrderByDescending(x => x.LastModified).ToList();
        var mF = _xmlService.GetState() == null ? null : _xmlService.GetState()!.ManagedFiles;
        var gD = _xmlProvider.GetGitState();
        var activeF = _lib.GetActiveFile();
        var vS = _xmlService.GetState() == null ? false : _xmlService.GetState()!.ValidState;

        var model = new XMLStateViewModel("Dateiübersicht", gD, hF, mF, vS) {
            ActiveFile = activeF,
            SyntaxCheck = _xmlService.GetSCCache(),
        };
        return View("~/Views/Admin/Dynamic/XMLState.cshtml", model);
    }

    private IActionResult error404() {
        Response.StatusCode = 404;
        return Redirect("/Error404");
    }
}
