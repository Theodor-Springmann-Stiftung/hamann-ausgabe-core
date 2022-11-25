using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using HaDocument.Models;
using System.Collections.Generic;
using System;
using HaDocument.Comparers;

namespace HaDocument.Reactors {
    class CommentReactor : Reactor {
        
        Dictionary<string, Comment> CreatedInstances;

        private bool _normalizeWhitespace = false;

        // State
        private string Type = "";
        private string Index = "";
        private string Lemma = "";
        private string Entry = "";
        private int Order = -1;
        private SortedDictionary<string, Comment> Subcomments;

        private bool subsection = false;
        private string subsectionIndex = "";
        private string subsectionLemma = "";
        private string subsectionEntry = "";
        private int subsectionOrder = -1;

        internal CommentReactor(IReader reader, IntermediateLibrary lib, bool normalizeWhitespace) : base(reader, lib) {
            _normalizeWhitespace = normalizeWhitespace;
            _lib.Comments = _lib.Comments ?? new Dictionary<string, Comment>();
            CreatedInstances = _lib.Comments;
            reader.OpenTag += Listen;
        }

        protected override void Listen(object _, Tag tag) {
            if (
                !tag.IsEmpty &&
                !tag.EndTag &&
                tag.Name == "kommcat" &&
                !string.IsNullOrWhiteSpace(tag["value"])
            ) {
                Type = tag["value"];
                Activate(_reader, tag);
            }
            else if (
                tag.EndTag &&
                tag.Name == "kommcat"
            ) {
                ResetType();
                Deactivate();
            }
        }

        protected override void Activate(IReader _, Tag tag) {
            _reader.Tag += OnTag;
        }

        private void OnTag(object _, Tag tag) {
            if (
                !tag.IsEmpty &&
                !tag.EndTag &&
                tag.Name == "kommentar" &&
                !string.IsNullOrWhiteSpace(tag["id"])
            ) {
                Index = tag["id"];
                Order = String.IsNullOrWhiteSpace(tag["sort"]) ? 0 : Int32.Parse(tag["sort"]);
            }
            else if (
                tag.EndTag &&
                tag.Name == "kommentar"
            ) {
                AddComment();
            }
            else if (
                !tag.EndTag &&
                !tag.IsEmpty &&
                tag.Name == "lemma"
            ) {
                _ = new ElementStringBinder(_reader, tag, AddLemma, _normalizeWhitespace);
            }
            else if (
                !tag.EndTag &&
                !tag.IsEmpty &&
                tag.Name == "eintrag"
                ) {
                _ = new ElementStringBinder(_reader, tag, AddEntry, _normalizeWhitespace);
            }
            else if (
                !tag.EndTag &&
                !tag.IsEmpty &&
                tag.Name == "subsection"
            ) {
                if (!String.IsNullOrWhiteSpace(tag["id"])) subsectionIndex = tag["id"];
                if (!String.IsNullOrWhiteSpace(tag["sort"])) subsectionOrder = Int32.Parse(tag["sort"]);
                if (Subcomments ==null) Subcomments = new SortedDictionary<string, Comment>();
                subsection = true;
            }
            else if (
                tag.EndTag &&
                tag.Name == "subsection"
            ) {
                subsection = false;
                AddSubsection();
            }
        }

        private void Deactivate() {
            _reader.Tag -= OnTag;
        }

        private void AddLemma(string str) {
            if (String.IsNullOrEmpty(str)) return;
            if (subsection) subsectionLemma = str;
            else Lemma = str;
        }

        private void AddEntry(string str) {
            if (String.IsNullOrWhiteSpace(str)) str = "";
            if (subsection) subsectionEntry = str;
            else Entry = str;
        }

        private void AddSubsection() {
            if (String.IsNullOrWhiteSpace(subsectionIndex)) return;
            if (String.IsNullOrWhiteSpace(subsectionLemma)) return;
            if (!Subcomments.ContainsKey(subsectionIndex))
            { 
                Subcomments.Add(subsectionIndex, new Comment(
                    subsectionEntry,
                    subsectionIndex,
                    Type,
                    subsectionLemma,
                    subsectionOrder,
                    null,
                    Index
                ));
            }
            ResetSubsection();
        }

        private void AddComment() {
            if (String.IsNullOrWhiteSpace(Index)) return;
            if (String.IsNullOrWhiteSpace(Lemma)) return;
            if (CreatedInstances.ContainsKey(Index)) return;
            CreatedInstances.Add(Index, new Comment(
                Entry,
                Index,
                Type,
                Lemma,
                Order,
                Subcomments
            ));
            Reset();
        }

        protected override void Reset() {
            Index = "";
            Lemma = "";
            Entry = "";
            Order = -1;
            Subcomments = null;
            ResetSubsection();
        }
        
        private void ResetSubsection() {
            subsection = false;
            subsectionEntry = "";
            subsectionIndex = "";
            subsectionLemma = "";
            subsectionOrder = -1;
        }

        private void ResetType() {
            Type = "";
        }
    }
}