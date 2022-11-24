namespace HaWeb.Controllers;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;

[Route("HKW/")]
public class HKWController : Controller {
    [Route("")]
    public IActionResult Index() {
        return View("~/Views/HKW/Static/Index.cshtml");
    }

    [Route("Baende/FliegenderBrief")]
    public IActionResult FliegenderBrief() {
        return View("~/Views/HKW/Static/FliegenderBrief.cshtml");
    }

    [Route("Baende/Kreuzzuege")]
    public IActionResult Kreuzzuege() {
        return View("~/Views/HKW/Static/Kreuzzuege.cshtml");
    }

    [Route("Baende/SokratischeDenkwuerdigkeiten")]
    public IActionResult SokratischeDenkwuerdigkeiten() {
        return View("~/Views/HKW/Static/SokratischeDenkwuerdigkeiten.cshtml");
    }

    [Route("Werkuebersicht")]
    public IActionResult Werkuebersicht() {
        return View("~/Views/HKW/Static/Werkuebersicht.cshtml");
    }

    [Route("Edition/Mitwirkende")]
    public IActionResult Mitwirkende() {
        return View("~/Views/HKW/Static/Mitwirkende.cshtml");
    }

    [Route("Edition/Editionsgeschichte")]
    public IActionResult Editionsgeschichte() {
        return View("~/Views/HKW/Static/Editionsgeschichte.cshtml");
    }
}