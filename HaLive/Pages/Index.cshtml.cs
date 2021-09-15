using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HaDocument.Interfaces;
using System.Collections.Immutable;
using HaDocument.Models;
using HaDocument.Comparers;
using HaXMLReader.EvArgs;
using HaLive.Models;
using HaXMLReader.Interfaces;
using System.Collections.Concurrent;

namespace HaLive.Pages
{
    public class IndexModel : PageModel
    {
        const int MAX_SRESULTS = 1000;
        const int MINLETTPG = 90;

        [BindProperty(SupportsGet = true)]
        public string fdletter { get; set; }= "";

        [BindProperty(SupportsGet = true)]
        public string search { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string person { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string havolume { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string hapage { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string selpage { get; set; } = "";

        public enum IndexState {
            Initial,
            Search,
            ZHLookup,
            PersonLookup
        }

        public enum IndexFailState {
            None,
            SearchTooManyResults,
            SearchNoResult,
            ZHNotFound,
            LetterNotFound
        }

        public int cleanpage { get; set; } = 0;

        public IndexState State { get; set; } = IndexState.Initial;
        public IndexFailState FailState { get; set; } = IndexFailState.None;
        public List<List<IGrouping<int, DocumentSearchResult>>> Result = null;
        public ImmutableSortedSet<Person> SelectedPersons = null;

        private ILibrary _lib;
        private IReaderService _readerService;

        public IndexModel(ILibrary lib, IReaderService readerservice) {
            _lib = lib;
            _readerService = readerservice;
            PrepareData();
        }

        private void PrepareData() {
            // Create a list of persons
            var _SelectedPersons = new List<Person>(100);
            foreach (var letter in _lib.MetasByDate) {
                foreach (var sender in letter.Senders) {
                    var p = _lib.Persons[sender];
                    if (!_SelectedPersons.Contains(p)) {
                        _SelectedPersons.Add(p);
                    }
                }
                foreach (var receiver in letter.Receivers) {
                    var p = _lib.Persons[receiver];
                    if (!_SelectedPersons.Contains(p)) {
                        _SelectedPersons.Add(p);
                    }
                }
            }
            SelectedPersons = _SelectedPersons.ToImmutableSortedSet(new PersonComparer());
        }

        public IActionResult OnGet() {
            if (!String.IsNullOrWhiteSpace(fdletter)) {
                if (_lib.Metas.Where(x => x.Value.Autopsic == fdletter).Any())
                    return Redirect("/Briefe/" + fdletter);
                else
                    FailState = IndexFailState.LetterNotFound;
                State = IndexState.Initial;
                return Default();
            }
            // State detection and Execution
            if (!String.IsNullOrWhiteSpace(search)) {
                State = IndexState.Search;
                return Search();
            }
            else if (!String.IsNullOrWhiteSpace(person)) {
                State = IndexState.PersonLookup;
                return PersonLookup();
            }
            else if (!String.IsNullOrWhiteSpace(havolume) ||
                     !String.IsNullOrWhiteSpace(hapage)) {
                State = IndexState.ZHLookup;
                return ZHLookup();
            }
            else {
                State = IndexState.Initial;
                return Default();
            }
        }
        
        internal ActionResult Search() {
            var ret = new ConcurrentBag<DocumentSearchResult>();
            var searchin = _lib.MetasByDate.Select(x => new DocumentSearchResult(x)).ToImmutableSortedSet(new LetterComparer());
            var cnt = 0; //TODO: Nicht in threads manipulieren
            Parallel.ForEach (searchin, (letter, state) => {
                if (cnt > 1000) {
                    FailState = IndexFailState.SearchTooManyResults;
                    state.Break();
                }
                var x = new DocumentSearch(letter, _readerService.RequestStringReader(_lib.Letters[letter.MetaData.Index].Element), search);
                cnt = cnt + x.Act().Results.Count;
                if (x._searchResult.Results.Count > 0 ) ret.Add(x._searchResult);
            });
            if (ret.Count() == 0) {
                FailState = IndexFailState.SearchNoResult;
            }
            return Execute(ret);
        }

        internal ActionResult ZHLookup() {
            var ret = ImmutableSortedSet.CreateBuilder(new LetterComparer());
            ImmutableDictionary<string, ImmutableDictionary<string, string>> pagedic;
            var selvolume = _lib.Structure.TryGetValue(havolume.Trim(), out pagedic);
            if (selvolume) {
                ImmutableDictionary<string, string> linedic;
                var selpage = pagedic.TryGetValue(hapage.Trim(), out linedic); 
                if (selpage) {
                    foreach (var entry in linedic) {
                        ret.Add(_lib.MetasByDate.Where(x => x.Index == entry.Value).Select(x => new DocumentSearchResult(x)).First());
                    }
                    if (ret.Count == 1) {
                        return Redirect("/Briefe/" + ret.First().MetaData.Autopsic + "#" + hapage + "-1");
                    }
                }
                else {
                    FailState = IndexFailState.ZHNotFound;
                    return Execute(ret);
                }
            }
            else {
                return FatalError();
            }
            return Execute(ret);
        }

        internal ActionResult PersonLookup() {
            return Execute(_lib.MetasByDate
                .Where(x => x.Senders.Contains(person) || x.Receivers.Contains(person))
                .Select(x => new DocumentSearchResult(x)));
        }

        internal ActionResult Default() {
            return Execute(_lib.MetasByDate
                .Select(x => new DocumentSearchResult(x)));
        }

        internal ActionResult Execute(IEnumerable<DocumentSearchResult> _result) {
            if (_result == null) FatalError();
            var res = _result.OrderBy(x => x, new LetterComparer()).ToLookup(x => x.MetaData.Sort.Year, x => x);
            return Paginate(res);
        }

        internal ActionResult Paginate(ILookup<int, DocumentSearchResult> _result) {
            if (_result == null) return Return();
            var curr = new List<IGrouping<int, DocumentSearchResult>>();
            Result = new List<List<IGrouping<int, DocumentSearchResult>>>();
            foreach (var elem in _result) {
                curr.Add(elem);
                if (curr.SelectMany(x => x).Count() > MINLETTPG || elem == _result.Last()) {
                    Result.Add(new List<IGrouping<int, DocumentSearchResult>>(curr));
                    curr.Clear();
                }
            }

            if (!String.IsNullOrWhiteSpace(selpage)) {
                int no;
                var isNo = Int32.TryParse(selpage, out no);
                if (!isNo ||
                    Result.Count < no || 
                    no <= 0) {
                    return FatalError();
                }
                else {
                    cleanpage = no - 1;
                }
            }
            else {
                selpage = "1";
            }
            return Return();
        }

        internal ActionResult FatalError() {
            Response.StatusCode = 404;
            return RedirectToPage("/Error");
        }

        internal ActionResult Return() {
            return Page();
        }
    }
}