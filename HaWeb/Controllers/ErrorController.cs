using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;

namespace HaWeb.Controllers;


public class ErrorController : Controller {
    [Route("Error404/")]
    public IActionResult ErrorNotFound() {
        return View();
    }
}