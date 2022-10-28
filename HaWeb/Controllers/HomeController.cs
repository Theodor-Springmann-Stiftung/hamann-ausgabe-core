using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;

namespace HaWeb.Controllers;

public class HomeController : Controller {

    [Route("")]
    [Route("Index")]
    public IActionResult Index() {
        return View("~/Views/Home/Index.cshtml");
    }

    [Route("Kontakt")]
    public IActionResult Kontakt() {
        return View("~/Views/Home/Kontakt.cshtml");
    }

    [Route("Datenschutzerklaerung")]
    public IActionResult Datenschutzerklaerung() {
        return View("~/Views/Home/Datenschutzerklaerung.cshtml");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
