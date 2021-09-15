using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HaDocument.Interfaces;
using HaDocument.Models;
using System.Collections.Concurrent;
using HaXMLReader.Interfaces;

namespace HaLive.Pages
{
    public class RegisterModel : PageModel
    {
        private ILibrary _lib;
        private IReaderService _readerService;

        [BindProperty(SupportsGet = true)]
        public string search { get; set; } = "";
        public bool maxSearch = false;

        [BindProperty(SupportsGet = true)]
        public string id { get; set; }

        public RegisterModel(ILibrary lib, IReaderService readerservice) {
            _lib = lib;
            _readerService = readerservice;
        }

        public IActionResult OnGet()
        {
            id = id.ToUpper();
            if (!_lib.CommentsByCategoryLetter["neuzeit"].Contains(id)) {
                Response.StatusCode = 404;
                return RedirectToPage("/Error");
            }
            return Page();
        }

        public IOrderedEnumerable<Comment> SelectComments(ILibrary lib) {
            if (!String.IsNullOrWhiteSpace(search))
                return Search(lib.CommentsByCategoryLetter["neuzeit"].SelectMany(x => x)).OrderBy(x => x.Index);
            return lib.CommentsByCategoryLetter["neuzeit"].Contains(id) ? lib.CommentsByCategoryLetter["neuzeit"][id].OrderBy(x => x.Index) : null;
        }

        private IEnumerable<Comment> Search(IEnumerable<Comment> all) {
            var ret = new ConcurrentBag<Comment>();
            var cnt = 0;
            Parallel.ForEach (all, (comm, state) => {
                if (cnt > 150) {
                    maxSearch = true;
                    state.Break();
                }
                if (SearchInComm(comm)) {
                    ret.Add(comm);
                    cnt++;
                }
            });
            return ret;
        }

        private bool SearchInComm(Comment comment) {
            var found = false;
            var x = new RegisterSearch(comment, _readerService.RequestStringReader(comment.Entry), search);
            found = x.Act();
            if (!found) {
                x = new RegisterSearch(comment, _readerService.RequestStringReader(comment.Lemma), search);
                found = x.Act();
            }
            if (comment.Kommentare != null)
                foreach (var sub in comment.Kommentare) {
                    if (!found) {
                        found = SearchInComm(sub.Value);
                    }
                    else break;
                }
            return found;
        }
    }
}