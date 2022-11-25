using System;
using HaXMLReader.EvArgs;
using HaXMLReader.Interfaces;
using HaDocument.Models;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HaDocument.Reactors {
    class AppDefsReactor : Reactor {
        internal Dictionary<string, App> CreatedInstances;

        // State
        private string Index;
        private string Name;
        private bool Category;

        internal AppDefsReactor(IReader reader, IntermediateLibrary lib) : base(reader, lib) {
            lib.Apps = new Dictionary<string, App>();
            CreatedInstances = lib.Apps;
            reader.Tag += Listen;
        }

        protected override void Listen(object sender, Tag tag) {
            if (
                !tag.EndTag && 
                tag.IsEmpty &&
                tag.Name == "appDef" && 
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
                if (!String.IsNullOrWhiteSpace(tag["category"] ))
                    Category = tag["category"] == "true" ? true : false;
                Add();
                _active = false;
            }
        }

        protected override void Reset() {
            Index = "";
            Name = "";
            Category = false;
        }

        protected void Add() {
            CreatedInstances.Add(Index, new App(Index, Name, Category));
        }
    }
}