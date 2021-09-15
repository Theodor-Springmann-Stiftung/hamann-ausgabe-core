using System;
using HaXMLReader.EvArgs;
using HaXMLReader.Interfaces;
using HaDocument.Models;
using System.Collections.Generic;
using System.Linq;

namespace HaDocument.Reactors {
    class MetaReactor : Reactor {

        internal Dictionary<string, Meta> CreatedInstances { get; }

        private string[] _availableVolumes;
        private (int, int) _availableYearRange;

        // State
        private string Index { get; set; } = "";
        private string Autopsic { get; set; } = "";
        private string Volume { get; set; } = "";
        private string Page { get; set; } = "";
        private string Date { get; set; } = "";
        private DateTime Sort { get; set; } = new DateTime(1700, 1, 1);
        private int Order { get; set; } = -1;
        private bool AltLineNumbering { get; set; } = false;
        private bool inZH { get; set; } = false;
        private OptionalBool hasOriginal { get; set; } = OptionalBool.None;
        private OptionalBool isProofread { get; set; } = OptionalBool.None;
        private OptionalBool isDraft { get; set; } = OptionalBool.None;
        private bool dateChanged {get; set; } = false;
        private string Location { get; set; } = "";
        private List<string> Senders { get; set; } = null;
        private List<string> Receivers { get; set; } = null;

        internal MetaReactor(IReader reader, IntermediateLibrary lib, string[] availableVolumes, (int, int) availableYearRange) : base(reader, lib) {
            _availableVolumes = availableVolumes;
            _availableYearRange = availableYearRange;
            lib.Metas = new Dictionary<string, Meta>();
            CreatedInstances = lib.Metas;
            reader.OpenTag += Listen;
        }

        protected override void Listen(object sender, Tag tag) {
            if (
                !tag.EndTag && 
                !tag.IsEmpty &&
                tag.Name =="letterDesc" && 
                !String.IsNullOrWhiteSpace(tag["ref"])
            ) {
                Activate(_reader, tag);
            }
        }

        protected override void Activate(IReader reader, Tag tag) {
            if (!_active && reader != null && tag != null) { 
                Reset();
                _active = true;
                Index = tag["ref"];
                reader.Tag += OnTag;
                _reader = reader;
            }
        }

        public void Deactivate() {
            if (_active) {
                Abort();
                Add();
            }
        }

        public void Abort() {
            if (_active) {
                _active = false;
                _reader.Tag -= OnTag;
            }
        }

        private void OnTag(object _, Tag tag) {
            switch (tag.Name)
            {
                case "autopsic":
                    Autopsic = tag["value"];
                    if (String.IsNullOrWhiteSpace(Autopsic)) Abort();
                    break;
                case "begin":
                    Page = tag["page"];
                    Volume = tag["vol"];
                    break;
                case "date":
                    Date = tag["value"];
                    break;
                case "location":
                    Location = tag["ref"];
                    break;
                case "sender":
                    if (!String.IsNullOrWhiteSpace(tag["ref"])) Senders.Add(tag["ref"]);
                    break;
                case "receiver":
                    if (!String.IsNullOrWhiteSpace(tag["ref"])) Receivers.Add(tag["ref"]);
                    break;
                case "sort":
                    DateTime res;
                    System.DateTime.TryParse(tag["value"], out res);
                    Sort = res;
                    int res2 = 0;
                    Int32.TryParse(tag["order"], out res2);
                    Order = res2;
                    break;
                case "hasOriginal":
                    var val = tag["value"];
                    if (val.ToLower() == "true")
                        hasOriginal = OptionalBool.True;
                    else if (val.ToLower() == "false")
                        hasOriginal = OptionalBool.False;
                    else
                        hasOriginal = OptionalBool.None;
                    break;
                case "isProofread":
                    var val2 = tag["value"];
                    if (val2.ToLower() == "true")
                        isProofread = OptionalBool.True;
                    else if (val2.ToLower() == "false")
                        isProofread = OptionalBool.False;
                    else
                        isProofread = OptionalBool.None;
                    break;
                case "isDraft":
                    var val3 = tag["value"];
                    if (val3.ToLower() == "true")
                        isDraft = OptionalBool.True;
                    else if (val3.ToLower() == "false")
                        isDraft = OptionalBool.False;
                    else
                        isDraft = OptionalBool.None;
                    break;
                case "ZHInfo":
                    if (!tag.EndTag) {
                        inZH = tag["inzh"] == "false" ? false : true;
                    }
                    break;
                case "dateChanged":
                    dateChanged = tag["value"].ToLower() == "true" ? true : false; 
                    break;
                case "alternativeLineNumbering":
                    AltLineNumbering =  tag["value"].ToLower() == "true" ? true : false;
                    break;
                case "letterDesc":
                    if (tag.EndTag) Deactivate();
                    break;
            }
        }

        private void Add() {
            if (
                _availableVolumes.Contains(Volume) ||
                (Sort.Year >= _availableYearRange.Item1 && Sort.Year <= _availableYearRange.Item2) ||
                (_availableVolumes == null && _availableYearRange.Item1 == 0 && _availableYearRange.Item2 == 0)
            ) {
                if (Index == "1190") {
                    ;
                }
                var ZHInfo = !inZH ? null : new ZHInfo(AltLineNumbering, dateChanged, Volume, Page);
                var meta = new Meta(
                    Index,
                    Autopsic,
                    Date,
                    Sort,
                    Order,
                    hasOriginal,
                    isProofread,
                    isDraft,
                    Location,
                    Senders,
                    Receivers,
                    ZHInfo
                );
                CreatedInstances.TryAdd(meta.Index, meta);
            }
        }

        protected override void Reset() {
            inZH = true;
            hasOriginal = OptionalBool.None;
            isProofread = OptionalBool.None;
            isDraft = OptionalBool.None;
            dateChanged = false;
            Index = "";
            Autopsic = "";
            Volume = "";
            Page = "";
            Date = "";
            DateTime Sort = new DateTime(1700, 1, 1);
            Order = -1;
            AltLineNumbering = false;
            Location = "";
            Senders = new List<string>();
            Receivers = new List<string>();
        }
    }
}