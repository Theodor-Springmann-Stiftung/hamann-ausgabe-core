using HaDocument.Models;
using System.Collections.Generic;
using HaXMLReader.EvArgs;
using HaXMLReader.Interfaces;
using System;

namespace HaDocument.Reactors {
    class EditreasonReactor : Reactor {
        internal Dictionary<string, Editreason> CreatedInstances;

        private Dictionary<string, string[]> _intermediateReasons = new Dictionary<string, string[]>();
        private bool _normalizeWhitespace = false;

        // State
        private string Index = "";

        private string letter = "";
        private string page = "";
        private string line = "";
        private Stack<string> index;

        private ElementStringBinder _element = null;

        internal EditreasonReactor(IReader reader, IntermediateLibrary lib, bool normalizeWhitespace) : base (reader, lib) {
            CreatedInstances = lib.Editreasons;
            index = new Stack<string>();
            _normalizeWhitespace = normalizeWhitespace;
            lib.Editreasons = new Dictionary<string, Editreason>();
            CreatedInstances = lib.Editreasons;
            reader.Tag += Listen;
            reader.ReadingStop += Scaffold;
        }

        protected void Scaffold(object _, EventArgs __) {
            foreach (var entry in _intermediateReasons) {
                if (
                    !String.IsNullOrWhiteSpace(entry.Value[0]) &&
                    !String.IsNullOrWhiteSpace(entry.Value[1]) &&
                    !String.IsNullOrWhiteSpace(entry.Value[2]) &&
                    !String.IsNullOrWhiteSpace(entry.Value[3])
                ) {
                    CreatedInstances.Add(
                        entry.Key,
                        new Editreason(
                            entry.Key,
                            entry.Value[0],
                            entry.Value[1],
                            entry.Value[2],
                            entry.Value[3],
                            entry.Value[5],
                            entry.Value[6],
                            entry.Value[4]
                        )
                    );
                }
            }
        }

        protected override void Listen(object sender, Tag tag) {
            if (
                !tag.EndTag && 
                !tag.IsEmpty &&
                tag.Name == "editreason" && 
                !String.IsNullOrWhiteSpace(tag["index"])
            ) {
                Activate(_reader, tag);
            }
            else if (
                !tag.EndTag &&
                !tag.IsEmpty &&
                tag.Name == "letterText"
            ) {
                letter = tag["letter"];
            }
            else if (
                !tag.EndTag &&
                !tag.IsEmpty &&
                tag.Name == "letterTradition"
            ) {
                letter = tag["letter"];
            }
            else if (
                !tag.EndTag &&
                tag.IsEmpty &&
                tag.Name == "page"
            ) {
                page = tag["index"];
            }
            else if (
                !tag.EndTag &&
                tag.IsEmpty &&
                tag.Name == "line"
            ) {
                line = tag["index"];
            }
            else if (
                !tag.EndTag &&
                tag.Name == "edit" &&
                !String.IsNullOrWhiteSpace(tag["ref"])
            ) {
                if (!_intermediateReasons.ContainsKey(tag["ref"])) {
                    _intermediateReasons.Add(tag["ref"], new string[7]);
                }
                _intermediateReasons[tag["ref"]][1] = letter;
                _intermediateReasons[tag["ref"]][2] = page;
                _intermediateReasons[tag["ref"]][3] = line;
                if (!tag.IsEmpty) {
                    index.Push(tag["ref"]);
                    new ElementStringBinder(_reader, tag, AddReference);
                }
            }
        }

        protected override void Activate(IReader reader, Tag tag) {
            if (!_active && reader != null && tag != null) {
                _active = true;
                _reader = reader;
                Index = tag["index"];
                _element = new ElementStringBinder(reader, tag, Add, _normalizeWhitespace);
            }
        }

        private void Add(string element) {
            if (String.IsNullOrWhiteSpace(element)) return;
            if (!_intermediateReasons.ContainsKey(Index)) {
                _intermediateReasons.Add(Index, new string[7]);
            }
            _intermediateReasons[Index][0] = element;
            Reset();
        }

        private void AddReference(string element) {
            if (String.IsNullOrWhiteSpace(element)) return;
            var ci = index.Pop();
            if (!_intermediateReasons.ContainsKey(ci)) {
                _intermediateReasons.Add(ci, new string[7]);
            }
            _intermediateReasons[ci][4] = element;
            _intermediateReasons[ci][5] = page;
            _intermediateReasons[ci][6] = line;
        }

        protected override void Reset() {
            Index = "";
            _active = false;
            _element = null;
        }

        protected void Deactivate() {
            _element.Unsubscribe();
            Reset();
        }
    }
}