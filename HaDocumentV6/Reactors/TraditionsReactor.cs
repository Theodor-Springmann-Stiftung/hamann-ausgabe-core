using HaDocument.Models;
using System.Collections.Generic;
using HaXMLReader.EvArgs;
using HaXMLReader.Interfaces;
using System;

namespace HaDocument.Reactors
{
    class TraditionsReactor : Reactor
    {
        internal Dictionary<string, Tradition> CreatedInstances;
        internal Dictionary<string, Dictionary<string, HashSet<string>>> CreatedStructure;
        internal Dictionary<string, List<Hand>> CreatedHands;
        private bool _normalizeWhitespace = false;

        // State
        private string ID = "";

        private string _page = "";
        private string _line = "";

        
        private List<Hand> _hands;
        private string _person = "";
        private string _handstartpg = "";
        private string _handstartln = "";

        private ElementStringBinder _element = null;

        internal TraditionsReactor(IReader reader, IntermediateLibrary lib, bool normalizeWhitespace) : base(reader, lib)
        {
            _normalizeWhitespace = normalizeWhitespace;
            _lib.Traditions = new Dictionary<string, Tradition>();
            if (lib.LetterPageLines == null)
                lib.LetterPageLines = new Dictionary<string, Dictionary<string, HashSet<string>>>();
            if (lib.Hands == null)
                lib.Hands = new Dictionary<string, List<Hand>>();
            CreatedHands = lib.Hands;
            CreatedInstances = _lib.Traditions;
            CreatedStructure = lib.LetterPageLines;
            reader.Tag += Listen;
        }

        protected override void Listen(object sender, Tag tag)
        {
            if (!tag.EndTag &&
                !tag.IsEmpty &&
                tag.Name == "letterTradition" &&
                !String.IsNullOrWhiteSpace(tag["letter"])
            )
            {
                Activate(_reader, tag);
            }
            else if (
                !tag.EndTag &&
                _active &&
                tag.Name == "ZHText"
            )
            {
                if (!CreatedStructure.ContainsKey(tag["letter"]))
                    this.CreatedStructure.Add(tag["letter"], new Dictionary<string, HashSet<string>>());
            }
            else if (
                !tag.EndTag &&
                _active &&
                tag.Name == "line" &&
                !String.IsNullOrWhiteSpace(tag["letter"])
            )
            {
                _line = tag["letter"];
                if (!CreatedStructure[ID][_page].Contains(_line))
                {
                    CreatedStructure[ID][_page].Add(_line);
                }
            }
            else if (
                !tag.EndTag &&
                _active &&
                tag.Name == "page" &&
                !String.IsNullOrWhiteSpace(tag["letter"])
            )
            {
                _page = tag["letter"];
                if (!CreatedStructure[ID].ContainsKey(_page))
                {
                    CreatedStructure[ID].Add(_page, new HashSet<string>());
                }
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
                _hands.Add(new Hand(ID, _person, _handstartpg, _handstartln, _page, _line));
            }
        }

        protected override void Activate(IReader reader, Tag tag)
        {
            if (!_active && reader != null && tag != null)
            {
                _active = true;
                ID = tag["letter"];
                _element = new ElementStringBinder(reader, tag, Add, _normalizeWhitespace);
            }
        }

        private void Add(string element)
        {
            if (String.IsNullOrWhiteSpace(element)) return;
            var reason = new Tradition(
                ID,
                element);
            CreatedInstances.TryAdd(ID, reason);
            if (_hands != null)
            {
                if (!CreatedHands.ContainsKey(ID))
                    CreatedHands.Add(ID, _hands);
                else
                    CreatedHands[ID].AddRange(_hands);
            }
            Reset();
        }

        protected override void Reset()
        {
            ID = "";
            _page = "";
            _line = "";
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