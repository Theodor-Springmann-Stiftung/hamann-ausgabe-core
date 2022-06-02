namespace HaWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using Microsoft.FeatureManagement.Mvc;

public class UpdateController : Controller
{
    // DI
    private ILibrary _lib;
    private IReaderService _readerService;

    public UpdateController(ILibrary lib, IReaderService readerService)
    {
        _lib = lib;
        _readerService = readerService;
    }


    [Route("Admin/Update")]
    [FeatureGate(Features.UpdateService)]
    public IActionResult Index()
    {
        return View("../Admin/Upload/Index");
    }
}