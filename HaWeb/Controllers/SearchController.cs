using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;

namespace HaWeb.Controllers;

public class SearchController : Controller
{

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
