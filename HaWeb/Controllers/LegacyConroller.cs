// Handles 304 redirects to links of the old page, so permalinks stay active.\
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;

namespace HaWeb.Controllers;


public class LegacyContoller : Controller {
    [Route("Supplementa/")]
    [Route("Supplementa/Register")]
    [Route("Supplementa/Register/{id?}")]
    public IActionResult SupplementaRegister(string? id) {
        if (id != null)
            return RedirectPermanent("/Register/Register/" + id);
        return RedirectPermanent("/Register/Register");
    }

    [Route("Supplementa/Bibelstellen")]
    public IActionResult SupplementaBibelstellen(string? id) {
        if (id != null)
            return RedirectPermanent("/Register/Bibelstellen/" + id);
        return RedirectPermanent("/Register/Bibelstellen");
    }

    [Route("Supplementa/Forschung")]
    public IActionResult SupplementaForschung(string? id) {
        if (id != null)
            return RedirectPermanent("/Register/Forschung/" + id);
        return RedirectPermanent("/Register/Forschung");
    }
}