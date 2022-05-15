using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;

namespace HaWeb.Controllers;

[Route("Register/[action]/{id?}")]
public class RegisterController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult Register(string? id)
    {
        return View("Index");
    }

    public IActionResult Bibelstellen(string? id)
    {
        return View("Index");
    }

    public IActionResult Forschung(string? id)
    {
        return View("Index");
    }
}