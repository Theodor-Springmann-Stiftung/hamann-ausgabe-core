using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;

namespace HaWeb.Controllers;

[Route("Edition/[action]")]
[Route("Edition/[action]/Index")]
public class EditionController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult Kontakt()
    {
        return View();
    }

    public IActionResult Mitwirkende()
    {
        return View();
    }

    public IActionResult Richtlinien()
    {
        return View();
    }

    public IActionResult Werkausgabe()
    {
        return View();
    }

    public IActionResult Editionsgeschichte()
    {
        return View();
    }
}