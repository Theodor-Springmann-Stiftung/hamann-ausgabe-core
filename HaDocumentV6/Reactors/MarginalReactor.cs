using HaDocument.Models;
using System.Collections.Generic;
using HaXMLReader.EvArgs;
using HaXMLReader.Interfaces;
using System;

namespace HaDocument.Reactors {
    class MarginalReactor : Reactor {
        internal Dictionary<string, List<Marginal>> CreatedInstances;
        internal Dictionary<string, List<Backlink>> CreatedBacklinks;
        private bool _normalizeWhitespace = false;

        // State
        private string? Sort = "";
        private string Letter = "";
        private string Page = "";
        private string Line = "";

        private ElementStringBinder _element = null;

        internal MarginalReactor(IReader reader, IntermediateLibrary lib, bool normalizeWhitespace) : base(reader, lib) {
            _normalizeWhitespace = normalizeWhitespace;
            lib.Marginals = new ();
            lib.Backlinks = new ();
            CreatedBacklinks = lib.Backlinks;
            CreatedInstances = lib.Marginals;
            reader.OpenTag += Listen;
        }

        protected override void Listen(object sender, Tag tag) {
            if (
                !tag.EndTag && 
                !tag.IsEmpty &&
                tag.Name == "marginal" &&
                !String.IsNullOrWhiteSpace(tag["letter"]) &&
                !String.IsNullOrWhiteSpace(tag["page"]) &&
                !String.IsNullOrWhiteSpace(tag["line"])
            ) {
                Activate(_reader, tag);
            }
        }

        protected override void Activate(IReader reader, Tag tag) {
            if (!_active && reader != null && tag != null) {
                _active = true;
                _reader = reader;
                Sort = tag["sort"];
                Letter = tag["letter"];
                Page = tag["page"];
                Line = tag["line"];
                _element = new ElementStringBinder(_reader, tag, Add, _normalizeWhitespace);
                reader.Tag += OnTag;
            }
        }

        private void OnTag(object _, Tag tag) {
            if(
                !tag.EndTag &&
                tag.Name =="link"
            ) {
                var id = "";
                if (tag.Values.ContainsKey("subref")) 
                    id = tag["subref"];
                else if (tag.Values.ContainsKey("ref"))
                    id = tag["ref"];
                if (!String.IsNullOrWhiteSpace(id)) {
                    if (!CreatedBacklinks.ContainsKey(id))
                        CreatedBacklinks.Add(id, new List<Backlink>());
                    CreatedBacklinks[id].Add(new Backlink(id, Letter, Page, Line));
                }
            }
        }

        private void Add(string element) {
            if (String.IsNullOrWhiteSpace(element)) return;
            var marg = new Marginal(
                Letter,
                Page,
                Line,
                String.IsNullOrWhiteSpace(Sort) ? null : Sort,
                element
            );
            if (!CreatedInstances.ContainsKey(Letter)) CreatedInstances.Add(Letter, new());
            CreatedInstances[Letter].Add(marg);
            Reset();
        }

        protected override void Reset() {
            Sort = "";
            _active = false;
            _element = null;
            _reader.Tag -= OnTag;
        }

        public void Deactivate() {
            _element.Unsubscribe();
            Reset();
        }
    }
}