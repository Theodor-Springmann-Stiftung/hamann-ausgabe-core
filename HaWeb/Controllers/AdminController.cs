namespace HaWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using Microsoft.FeatureManagement.Mvc;
using HaWeb.FileHelpers;
public class AdminController : Controller {
    // DI
    private IHaDocumentWrappper _lib;
    private IReaderService _readerService;

    public AdminController(IHaDocumentWrappper lib, IReaderService readerService) {
        _lib = lib;
        _readerService = readerService;
    }


    [Route("Admin")]
    [FeatureGate(Features.AdminService)]
    public IActionResult Index() {
        return Redirect("/Admin/XMLState");
    }
}