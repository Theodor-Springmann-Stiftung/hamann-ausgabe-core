using HaDocument.Models;
using HaDocument.Interfaces;
using HaDocument.Comparers;
using HaXMLReader.Interfaces;
using HaLive.Models;
using HaXMLReader.EvArgs;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace HaLive {
    internal class DocumentSearch {
        internal DocumentSearchResult _searchResult { get; }
        private IReader _reader { get; }
        private string _search { get; }

        internal DocumentSearch(DocumentSearchResult res, IReader reader, string search) {
            _searchResult = res;
            _reader = reader;
            _search = search;
            _pg = "";
        }

        internal DocumentSearchResult Act() {
            _reader.Text += OnText;
            _reader.SingleTag += OnSTag;
            _reader.Read();
            return _searchResult;
        }

        internal void OnText(object _, Text text) {
            if (text.Value.ToLower().Contains(_search.ToLower())) {
                _searchResult.Results.Add(new DocumentResult(text.Value, _pg, _ln));
            }
        }

        private string _pg;
        private string _ln;

        internal void OnSTag(object _, Tag tag) {
            if (tag.Name == "page")
                _pg = tag["index"];
            else if (tag.Name == "line")
                _ln = tag["index"];
        }
    }


    internal class RegisterSearch {
        internal Comment _searchResult { get; }
        private IReader _reader { get; }
        private string _search { get; }
        private bool found;

        internal RegisterSearch(Comment res, IReader reader, string search) {
            _searchResult = res;
            _reader = reader;
            _search = search;
            found = false;
        }

        internal bool Act() {
            _reader.Text += OnText;
            _reader.Read();
            return found;
        }

        internal void OnText(object _, Text text) {
            if (text.Value.ToLower().Contains(_search.ToLower())) {
                found = true;
            }
        }
    }
}