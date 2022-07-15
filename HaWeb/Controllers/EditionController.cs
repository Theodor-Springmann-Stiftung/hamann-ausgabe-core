using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;

namespace HaWeb.Controllers;

[Route("Edition/[action]")]
public class EditionController : Controller {
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult Kontakt() {
        return View("~/Views/HKB/Static/Kontakt.cshtml");
    }

    public IActionResult Mitwirkende() {
        return View("~/Views/HKB/Static/Mitwirkende.cshtml");
    }

    public IActionResult Richtlinien() {
        return View("~/Views/HKB/Static/Richtlinien.cshtml");
    }

    public IActionResult Werkausgabe() {
        return View("~/Views/HKB/Static/Werkausgabe.cshtml");
    }

    public IActionResult Editionsgeschichte() {
        return View("~/Views/HKB/Static/Editionsgeschichte.cshtml");
    }
}