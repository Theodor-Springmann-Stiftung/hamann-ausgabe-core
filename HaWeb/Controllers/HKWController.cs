namespace HaWeb.Controllers;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;

[Route("HKW/")]
public class HKWController : Controller {
    [Route("Start")]
    public IActionResult Index() {
        return View("~/Views/HKW/Static/Index.cshtml");
    }

    [Route("Erschienen/FliegenderBrief")]
    public IActionResult FliegenderBrief() {
        return View("~/Views/HKW/Static/FliegenderBrief.cshtml");
    }

    [Route("Geplant/Kreuzzuege")]
    public IActionResult Kreuzzuege() {
        return View("~/Views/HKW/Static/Kreuzzuege.cshtml");
    }

    [Route("Erschienen/SokratischeDenkwuerdigkeiten")]
    public IActionResult SokratischeDenkwuerdigkeiten() {
        return View("~/Views/HKW/Static/SokratischeDenkwuerdigkeiten.cshtml");
    }

    [Route("Uebersicht")]
    public IActionResult Uebersicht() {
        return View("~/Views/HKW/Static/Uebersicht.cshtml");
    }
}