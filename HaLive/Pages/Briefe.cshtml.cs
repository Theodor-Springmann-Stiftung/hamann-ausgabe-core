using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HaDocument.Interfaces;
using HaDocument.Models;
using System;


namespace HaLive.Pages
{
    public class BriefeModel : PageModel
    {
        internal ILibrary _lib;
        
        [BindProperty(SupportsGet = true)]
        public string id { get; set; }

        public BriefeModel(ILibrary lib) {
            _lib = lib;
        }

        public IActionResult OnGet()
        {
            if (String.IsNullOrWhiteSpace(id)) {
                return RedirectPermanent("/Briefe/1");
            }
            var res = _lib.Metas.Where(x => x.Value.Autopsic == id);
            if (!res.Any() || !_lib.Metas.ContainsKey(res.First().Key))  {
                Response.StatusCode = 404;
                return RedirectToPage("/Error");
            }
            _lib.Metas.Where(x => x.Value.Autopsic == id).First();
            return Page();
        }
    }
}