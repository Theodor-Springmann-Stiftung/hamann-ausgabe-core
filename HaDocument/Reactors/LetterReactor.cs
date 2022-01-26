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
        private string Index = "";
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
                !String.IsNullOrWhiteSpace(tag["index"])
            )
            {
                Activate(_reader, tag);
                if (!CreatedStructure.ContainsKey(tag["index"]))
                    this.CreatedStructure.Add(tag["index"], new Dictionary<string, HashSet<string>>());
            }
            else if (
                !tag.EndTag &&
                _active &&
                tag.Name == "line" &&
                !String.IsNullOrWhiteSpace(tag["index"])
            )
            {
                _line = tag["index"];
                if (!CreatedStructure[Index][_page].Contains(_line))
                {
                    CreatedStructure[Index][_page].Add(_line);
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
                if (!CreatedStructure[Index].ContainsKey(_page))
                {
                    CreatedStructure[Index].Add(_page, new HashSet<string>());
                }
            }
            else if (
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
                tag.EndTag &&
                tag.Name == "hand"
            )
            {
                if (_hands == null)
                {
                    _hands = new List<Hand>();
                }
                _hands.Add(new Hand(Index, _person, _handstartpg, _handstartln, _page, _line));
            }
        }

        protected override void Activate(IReader reader, Tag tag)
        {
            if (!_active && reader != null && tag != null)
            {
                _active = true;
                _reader = reader;
                Index = tag["index"];
                _element = new ElementStringBinder(reader, tag, Add, _normalizeWhitespace);
            }
        }

        private void Add(string text)
        {
            if (String.IsNullOrWhiteSpace(text)) return;
            var letter = new Letter(
                Index,
                text
            );
            CreatedInstances.TryAdd(Index, letter);
            if (_hands != null)
            {
                if (!CreatedHands.ContainsKey(Index))
                    CreatedHands.Add(Index, _hands);
                else
                    CreatedHands[Index].AddRange(_hands);
            }
            Reset();
        }

        protected override void Reset()
        {
            Index = "";
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