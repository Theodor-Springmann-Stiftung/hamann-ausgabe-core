using System;
using HaXMLReader.EvArgs;
using HaXMLReader.Interfaces;
using HaDocument.Models;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;

namespace HaDocument.Reactors
{
    class LetterReactor : Reactor
    {
        internal Dictionary<string, Letter> CreatedInstances;
        internal Dictionary<string, List<Hand>> CreatedHands;
        internal Dictionary<string, Dictionary<string, HashSet<string>>> CreatedStructure;

        // State
        private string Letter = "";
        private ElementStringBinder _element = null;

        private bool _normalizeWhitespace = false;
        private string _page = "";
        private string _line = "";
        private List<Hand> _hands;
        private string _person = "";
        private string _handstartpg = "";
        private string _handstartln = "";

        internal LetterReactor(IReader reader, IntermediateLibrary lib, bool normalizeWhitespace) : base(reader, lib)
        {
            _normalizeWhitespace = normalizeWhitespace;
            lib.Letters = new Dictionary<string, Letter>();
            if (lib.Hands == null)
                lib.Hands = new Dictionary<string, List<Hand>>();
            if (lib.LetterPageLines == null)
                lib.LetterPageLines = new Dictionary<string, Dictionary<string, HashSet<string>>>();
            CreatedHands = lib.Hands;
            CreatedInstances = lib.Letters;
            CreatedStructure = lib.LetterPageLines;
            reader.OpenTag += Listen;
            reader.SingleTag += Listen;
            reader.CloseTag += Listen;
        }

        protected override void Listen(object sender, Tag tag)
        {
            if (
                !tag.EndTag &&
                !tag.IsEmpty &&
                tag.Name == "letterText" &&
                !String.IsNullOrWhiteSpace(tag["letter"])
            )
            {
                Activate(_reader, tag);
                if (!CreatedStructure.ContainsKey(tag["letter"]))
                    this.CreatedStructure.Add(tag["letter"], new Dictionary<string, HashSet<string>>());
            }
            else if (
                !tag.EndTag &&
                _active &&
                tag.Name == "line" &&
                !String.IsNullOrWhiteSpace(tag["index"])
            )
            {
                _line = tag["index"];
                if (!CreatedStructure[Letter][_page].Contains(_line))
                {
                    CreatedStructure[Letter][_page].Add(_line);
                }
            }
            else if (
                !tag.EndTag &&
                _active &&
                tag.Name == "page" &&
                !String.IsNullOrWhiteSpace(tag["index"])
            )
            {
                _page = tag["index"];
                if (!CreatedStructure[Letter].ContainsKey(_page))
                    CreatedStructure[Letter].Add(_page, new HashSet<string>());
            }
            else if (
                _active &&
                !tag.EndTag &&
                !tag.IsEmpty &&
                tag.Name == "hand" &&
                !String.IsNullOrWhiteSpace(tag["ref"])
            )
            {
                _person = tag["ref"];
                _handstartln = _line;
                _handstartpg = _page;
            }
            else if (
                _active &&
                tag.EndTag &&
                tag.Name == "hand"
            )
            {
                if (_hands == null)
                    _hands = new List<Hand>();
                _hands.Add(new Hand(Letter, _person, _handstartpg, _handstartln, _page, _line));
            }
        }

        protected override void Activate(IReader reader, Tag tag)
        {
            if (!_active && reader != null && tag != null)
            {
                _active = true;
                _reader = reader;
                Letter = tag["letter"];
                _element = new ElementStringBinder(reader, tag, Add, _normalizeWhitespace);
            }
        }

        private void Add(string text)
        {
            if (String.IsNullOrWhiteSpace(text)) return;
            var letter = new Letter(
                Letter,
                text
            );
            CreatedInstances.TryAdd(Letter, letter);
            if (_hands != null)
            {
                if (!CreatedHands.ContainsKey(Letter))
                    CreatedHands.Add(Letter, _hands);
                else
                    CreatedHands[Letter].AddRange(_hands);
            }
            Reset();
        }

        protected override void Reset()
        {
            Letter = "";
            _active = false;
            _element = null;
            _hands = null;
        }

        protected void Deactivate()
        {
            _element.Unsubscribe();
            Reset();
        }

    }
}