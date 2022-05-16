using System;
using HaXMLReader.EvArgs;
using HaXMLReader.Interfaces;
using HaDocument.Models;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HaDocument.Reactors {
    class PersonDefsReactor : Reactor {
        internal Dictionary<string, Person> CreatedInstances;

        // State
        private string Index;
        private string Name;
        private string Prename = "";
        private string Surname = "";

        internal PersonDefsReactor(IReader reader, IntermediateLibrary lib) : base(reader, lib) {
            lib.Persons = new Dictionary<string, Person>();
            CreatedInstances = lib.Persons;
            reader.Tag += Listen;
        }

        protected override void Listen(object sender, Tag tag) {
            if (!tag.EndTag && 
                tag.IsEmpty &&
                tag.Name == "personDef" && 
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
                Prename = tag["vorname"];
                Surname = tag["nachname"];
                Add();
                _active = false;
            }
        }

        protected override void Reset() {
            Index = "";
            Name = "";
            Prename = "";
            Surname = "";
        }

        public void Add() {
            CreatedInstances.Add(Index, new Person(Index, Name, Prename, Surname));
        }
    }
}