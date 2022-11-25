using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HaDocument.Interfaces;

namespace HaLive.Pages
{
    public class BibelstellenModel : PageModel
    {
        private ILibrary _lib;

        [BindProperty(SupportsGet = true)]
        public string id { get; set; }

        public BibelstellenModel(ILibrary lib) {
            _lib = lib;
        }

        public IActionResult OnGet()
        {
            if (!_lib.CommentsByCategory["bibel"].ToLookup(x => x.Index.Substring(0,2).ToUpper()).Contains(id.ToUpper())) {
                Response.StatusCode = 404;
                return RedirectToPage("/Error");
            }
            return Page();
        }
    }
}