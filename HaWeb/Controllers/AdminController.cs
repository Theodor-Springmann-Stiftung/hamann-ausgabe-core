namespace HaWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using Microsoft.FeatureManagement.Mvc;

public class AdminController : Controller {
    // DI
    private ILibrary _lib;
    private IReaderService _readerService;

    public AdminController(ILibrary lib, IReaderService readerService) {
        _lib = lib;
        _readerService = readerService;
    }


    [Route("Admin")]
    [FeatureGate(Features.AdminService)]
    public IActionResult Index() {
        return Redirect("/Admin/Upload");
    }
}