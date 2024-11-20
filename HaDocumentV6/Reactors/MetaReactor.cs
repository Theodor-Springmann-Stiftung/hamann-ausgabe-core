using System;
using HaXMLReader.EvArgs;
using HaXMLReader.Interfaces;
using HaDocument.Models;
using System.Collections.Generic;
using System.Linq;

namespace HaDocument.Reactors {
    class MetaReactor : Reactor {

        internal Dictionary<string, Meta> CreatedInstances { get; }
        internal Dictionary<string, Meta> ExcludedInstances { get; }
        private string[] _availableVolumes;
        private (int, int) _availableYearRange;

        // State
        private string ID { get; set; } = "";
        private string Volume { get; set; } = "";
        private string Page { get; set; } = "";
        private string Date { get; set; } = "";
        private DateTime Sort { get; set; } = new DateTime(1700, 1, 1);
        private int Order { get; set; } = -1;
        private bool AltLineNumbering { get; set; } = false;
        private bool inZH { get; set; } = false;
        private bool? hasOriginal { get; set; } = null;
        private bool? isProofread { get; set; } = null;
        private bool? isDraft { get; set; } = null;
        private AdditionalDates? AdditionalDates { get; set; } = null;
        private bool dateChanged { get; set; } = false;
        private string Location { get; set; } = "";
        private List<string> Senders { get; set; } = null;
        private List<string> Receivers { get; set; } = null;

        internal MetaReactor(IReader reader, IntermediateLibrary lib, string[] availableVolumes, (int, int) availableYearRange) : base(reader, lib) {
            _availableVolumes = availableVolumes;
            _availableYearRange = availableYearRange;
            lib.Metas = new Dictionary<string, Meta>();
            CreatedInstances = lib.Metas;
            lib.ExcludedMetas = new Dictionary<string, Meta>();
            ExcludedInstances = lib.ExcludedMetas;
            reader.OpenTag += Listen;
        }

        protected override void Listen(object sender, Tag tag) {
            if (
                !tag.EndTag &&
                !tag.IsEmpty &&
                tag.Name == "letterDesc" &&
                !String.IsNullOrWhiteSpace(tag["letter"])
            ) {
                Activate(_reader, tag);
            }
        }

        protected override void Activate(IReader reader, Tag tag) {
            if (!_active && reader != null && tag != null) {
                Reset();
                _active = true;
                ID = tag["letter"];
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
            switch (tag.Name) {
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

                    AdditionalDates = new AdditionalDates(
                        GetDateTime(tag["notbefore"]),
                        GetDateTime(tag["notafter"]),
                        GetDateTime(tag["from"]),
                        GetDateTime(tag["to"]),
                        tag["cert"]
                    );
                    break;
                case "hasOriginal":
                    var val = tag["value"];
                    if (val.ToLower() == "true")
                        hasOriginal = true;
                    else if (val.ToLower() == "false")
                        hasOriginal = false;
                    else
                        hasOriginal = null;
                    break;
                case "isProofread":
                    var val2 = tag["value"];
                    if (val2.ToLower() == "true")
                        isProofread = true;
                    else if (val2.ToLower() == "false")
                        isProofread = false;
                    else
                        isProofread = null;
                    break;
                case "isDraft":
                    var val3 = tag["value"];
                    if (val3.ToLower() == "true")
                        isDraft = true;
                    else if (val3.ToLower() == "false")
                        isDraft = false;
                    else
                        isDraft = null;
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
                    AltLineNumbering = tag["value"].ToLower() == "true" ? true : false;
                    break;
                case "letterDesc":
                    if (tag.EndTag) Deactivate();
                    break;
            }
        }

        private void Add() {
            var ZHInfo = !inZH ? null : new ZHInfo(AltLineNumbering, dateChanged, Volume, Page);
            if (AdditionalDates != null) {
                if (AdditionalDates.NotBefore == null && AdditionalDates.NotAfter == null && AdditionalDates.From == null && AdditionalDates.To == null && AdditionalDates.Cert == null) {
                    AdditionalDates = null;
                }
            }
            var meta = new Meta(
                    ID,
                    Date,
                    Sort,
                    Order,
                    hasOriginal,
                    isProofread,
                    isDraft,
                    Location,
                    Senders,
                    Receivers,
                    ZHInfo,
                    AdditionalDates
                );
            if (
                _availableVolumes.Contains(Volume) ||
                (Sort.Year >= _availableYearRange.Item1 && Sort.Year <= _availableYearRange.Item2) ||
                (_availableVolumes == null && _availableYearRange.Item1 == 0 && _availableYearRange.Item2 == 0)
            ) {
                CreatedInstances.TryAdd(meta.ID, meta);
            }
            else {
                ExcludedInstances.TryAdd(meta.ID, meta);
            }
        }

        protected override void Reset() {
            inZH = true;
            hasOriginal = null;
            isProofread = null;
            isDraft = null;
            dateChanged = false;
            AdditionalDates = null;
            ID = "";
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

        private DateTime? GetDateTime(string date) {
            DateTime res;
            var ret = System.DateTime.TryParse(date, out res);
            if (ret) return res;
            else return null;
        }
    }
}
