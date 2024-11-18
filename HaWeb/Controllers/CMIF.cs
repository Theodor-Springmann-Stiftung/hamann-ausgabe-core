namespace HaWeb.Controllers;

using HaWeb.CMIF;
using HaWeb.FileHelpers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class CMIFController : Controller {

    private IHaDocumentWrappper _lib;

    public CMIFController(IHaDocumentWrappper lib) {
        _lib = lib;
    }

    [Route("HKB/CMIF")]
    [HttpGet]
    [Produces("application/xml")]
    public IActionResult CMIF() {
        var lib = _lib.GetLibrary();
        if (lib != null) {
            var document = new TeiDocument(lib);
            return Ok(document);
        }
        return NotFound();
    }
}
