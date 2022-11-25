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
    public class ForschungModel : PageModel
    {
        private ILibrary _lib;
        private IReaderService _readerService;

        [BindProperty(SupportsGet = true)]
        public string search { get; set; } = "";
        public bool maxSearch = false;

        [BindProperty(SupportsGet = true)]
        public string id { get; set; }

        public ForschungModel(ILibrary lib, IReaderService readerService) {
            _lib = lib;
            _readerService = readerService;
        }

        public IActionResult OnGet()
        {
            id = id.ToUpper();
            if (!_lib.CommentsByCategoryLetter["forschung"].Contains(id) && id != "EDITIONEN") {
                Response.StatusCode = 404;
                return RedirectToPage("/Error");
            }
            return Page();
        }

        public IOrderedEnumerable<Comment> SelectComments() {
            if (!String.IsNullOrWhiteSpace(search))
                return Search(_lib.CommentsByCategory["forschung"]).OrderBy(x => x.Index);
            if (id == "EDITIONEN")
                return _lib.CommentsByCategory["editionen"].OrderBy(x => x.Index);
            return _lib.CommentsByCategoryLetter["forschung"][id].OrderBy(x => x.Index);
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