// Handles 304 redirects to links of the old page, so permalinks stay active.\
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;
namespace HaWeb.Controllers;

public class LegacyContoller : Controller {
    // Umbennenung von Supplementa zu Register, Verschiebung der Edition nach /HKB
    [Route("Supplementa/")]
    [Route("Supplementa/Register")]
    [Route("Supplementa/Register/{id?}")]
    public IActionResult SupplementaRegister(string? id) {
        if (id != null)
            return RedirectPermanent("/HKB/Register/Register/" + id);
        return RedirectPermanent("/HKB/Register/Register");
    }

    [Route("Supplementa/Bibelstellen")]
    public IActionResult SupplementaBibelstellen(string? id) {
        if (id != null)
            return RedirectPermanent("/HKB/Register/Bibelstellen/" + id);
        return RedirectPermanent("/HKB/Register/Bibelstellen");
    }

    [Route("Supplementa/Forschung")]
    public IActionResult SupplementaForschung(string? id) {
        if (id != null)
            return RedirectPermanent("/HKB/Register/Forschung/" + id);
        return RedirectPermanent("/HKB/Register/Forschung");
    }

    // Verschiebung der Edition nach /HKB/
    [Route("/Edition/Mitwirkende")]
    public IActionResult Mitwirkende() {
        return RedirectPermanent("/HKB/Edition/Mitwirkende");
    }

    [Route("/Edition/Richtlinien")]
    public IActionResult Richtlinien() {
        return RedirectPermanent("/HKB/Edition/Richtlinien");
    }

    [Route("/Edition/Editionsgeschichte")]
    public IActionResult Editionsgeschichte() {
        return RedirectPermanent("/HKB/Edition/Editionsgeschichte");
    }

    [Route("/Briefe")]
    [Route("/Briefe/{id?}")]
    public IActionResult Briefe(string? id) {
        if (id != null)
            return RedirectPermanent("/HKB/Briefe/" + id);
        return RedirectPermanent("/HKB/Briefe");
    }

    // Verschiebung der Werkausgabe nach /HKW/
    [Route("/Edition/Werkausgabe")]
    public IActionResult Werkausgabe() {
        return RedirectPermanent("/HKW/Start");
    }

    // Verschiebung von Kontakt nach /
    [Route("/Edition/Kontakt")]
    public IActionResult Kontakt() {
        return RedirectPermanent("/Kontakt");
    }
}