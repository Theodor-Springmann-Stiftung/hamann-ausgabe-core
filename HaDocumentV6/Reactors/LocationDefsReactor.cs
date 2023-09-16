using System;
using HaXMLReader.EvArgs;
using HaXMLReader.Interfaces;
using HaDocument.Models;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HaDocument.Reactors {
    class LocationDefsReactor : Reactor {
        internal Dictionary<string, Location> CreatedInstances;


        // State
        private string Index;
        private string Name;
        private string? Reference;

        internal LocationDefsReactor(IReader reader, IntermediateLibrary lib) : base(reader, lib) {
            lib.Locations = new Dictionary<string, Location>();
            CreatedInstances = lib.Locations;
            reader.Tag += Listen;
        }

        protected override void Listen(object sender, Tag tag) {
            if (
                !tag.EndTag && 
                tag.IsEmpty &&
                tag.Name == "locationDef" && 
                !String.IsNullOrWhiteSpace(tag["index"]) &&
                !String.IsNullOrWhiteSpace(tag["name"])
            ) {
                Activate(_reader, tag);
            }
        }

        protected override void Activate(IReader reader, Tag tag) {
            if (!_active && reader != null && tag != null) {
                Reset();
                _active = true;
                Index = tag["index"];
                Name = tag["name"];
                Reference = String.IsNullOrWhiteSpace(tag["ref"]) ? null : tag["ref"];
                Add();
                _active = false;
            }
        }

        protected override void Reset() {
            Index = "";
            Name = "";
            Reference = null;
        }

        protected void Add() {
            CreatedInstances.Add(Index, new Location(Index, Name, Reference));
        }
    }
}