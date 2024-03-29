using HaDocument.Interfaces;
using HaDocument.Models;
using System;
using System.Text;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using HaXMLReader;
using System.Collections.Generic;

namespace HaLive {
    public class LinkBuilder {
        private ILibrary _lib;
        private IReader _reader;
        private StringBuilder _sb;

        private bool _followlinksinchildren;

        private bool _followlinksinthis;

        public LinkBuilder(ILibrary lib, IReader reader, StringBuilder stringBuilder, bool followlinksinchildren = true, bool followlinksinthis = true) {
            if (lib == null || reader == null || stringBuilder == null) throw new ArgumentNullException();
            _lib = lib;
            _reader = reader;
            _sb = stringBuilder;
            _followlinksinchildren = followlinksinchildren;
            _followlinksinthis = followlinksinthis;
            reader.Tag += OnTag;
        }

        private void OnTag(object _, Tag tag) {
            if (tag.Name == "wwwlink" || tag.Name == "intlink" || tag.Name == "link") {
                if (tag.EndTag && _followlinksinthis) {
                    _sb.Append(HTMLHelpers.CreateEndElement("a"));
                }
                else {
                    if (tag.Name == "wwwlink" && tag.Values.ContainsKey("address") && _followlinksinthis)
                        _sb.Append(HTMLHelpers.CreateCustomElement("a", 
                            new HaLive.Attribute() { Name = "class", Value = "hlink wwwlink invlink" }, 
                            new HaLive.Attribute() { Name = "href", Value = tag["address"]},
                            new HaLive.Attribute() { Name = "target", Value = "_blank"},
                            new HaLive.Attribute() { Name = "rel", Value = "noopener noreferrer"}));
                    if (tag.Name == "intlink" && tag.Values.ContainsKey("letter") && _lib.Metas.ContainsKey(tag["letter"])) {
                        var letter = _lib.Metas[tag["letter"]];
                        _sb.Append(HTMLHelpers.CreateElement("a", "hlink intlink invlink", "/Briefe/" + letter.Autopsic + "#" + tag["page"] + "-" + tag["line"]));
                        if (!tag.Values.ContainsKey("linktext") || tag.Values["linktext"] == "true") {
                            var linkstring = "";
                            var ZHstring = "";
                            var pglnstring= "";
                            linkstring += "HKB&nbsp;" + letter.Autopsic;
                            if (tag.Values.ContainsKey("page")) {
                                pglnstring += tag["page"];
                                if (tag.Values.ContainsKey("line")) {
                                    pglnstring += "/" + tag["line"];
                                }
                                if (letter.ZH != null)
                                    ZHstring += HTMLHelpers.ToRoman(Int32.Parse(letter.ZH.Volume)) + "&nbsp;";
                                linkstring += "&nbsp;(&#8239;";
                                linkstring += ZHstring;
                                linkstring += pglnstring;
                                linkstring += "&#8239;)";
                            }
                            _sb.Append(linkstring);
                        }
                    }
                    if (tag.Name == "link" && tag.Values != null) {
                        Comment comment = null;
                        if (tag.Values.ContainsKey("subref") && _lib.SubCommentsByID.ContainsKey(tag["subref"])) 
                            comment = _lib.SubCommentsByID[tag["subref"]];
                        else if (tag.Values.ContainsKey("ref"))
                            if (_lib.Comments.ContainsKey(tag["ref"]))
                                comment = _lib.Comments[tag["ref"]];
                            else if (_lib.SubCommentsByID.ContainsKey(tag["ref"]))
                                comment = _lib.SubCommentsByID[tag["ref"]];
                        if (comment != null) {
                            var linkloc = String.IsNullOrWhiteSpace(comment.Parent) ? comment.Index : comment.Parent;
                            if (_followlinksinthis)
                                if (comment.Type == "neuzeit") 
                                    _sb.Append(HTMLHelpers.CreateElement("a", "hlink link invlink", "/Supplementa/Register/" + linkloc[0] + "#" + comment.Index));
                                else if (comment.Type == "bibel")
                                    _sb.Append(HTMLHelpers.CreateElement("a", "hlink link invlink", "/Supplementa/Bibelstellen/" + linkloc[0] + linkloc[1] + "#" + comment.Index));
                                else if (comment.Type == "forschung")
                                    _sb.Append(HTMLHelpers.CreateElement("a", "hlink link invlink", "/Supplementa/Forschung/" + linkloc[0] + "#" + comment.Index));
                            _sb.Append(GetLemmaString(tag, comment));
                        }
                    }
                   if (tag.IsEmpty && _followlinksinthis) _sb.Append(HTMLHelpers.CreateEndElement("a"));
                }
            }
        }

        private string GetLemmaString(Tag tag, Comment comment) {
            if (!tag.Values.ContainsKey("linktext") || tag.Values["linktext"] == "true") {
                var sb = new StringBuilder();
                var subreader = new UTF8StringReader(comment.Lemma);
                new LinkBuilder(_lib, subreader, sb, _followlinksinchildren, _followlinksinchildren);
                List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> OTag_Funcs = new List<(Func<Tag, bool>, Action<StringBuilder, Tag>)>() {
                    ( x => x.Name == "lemma", (strbd, x) => strbd.Append(HTMLHelpers.CreateElement("div", "reference")) ),
                    ( x => x.Name == "titel", (strbd, x) => strbd.Append(HTMLHelpers.CreateElement("span", "title")) )
                };
                List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> CTag_Funcs = new List<(Func<Tag, bool>, Action<StringBuilder, Tag>)>() {
                    ( x => x.Name == "lemma", (strbd, x) => strbd.Append(HTMLHelpers.CreateEndElement("div")) ),
                    ( x => x.Name == "titel", (strbd, x) => strbd.Append(HTMLHelpers.CreateEndElement("span")) )
                };
                List<(Func<Text, bool>, Action<StringBuilder, Text>)> Text_Funcs = new List<(Func<Text, bool>, Action<StringBuilder, Text>)>() {
                    ( x => true, (strbd, txt) => strbd.Append(txt.Value))
                };
                new StandardSubscriber(subreader, sb, OTag_Funcs, null, CTag_Funcs, Text_Funcs, null);
                subreader.Read();
                return sb.ToString();
            }
            return "";
        }

        public void Dispose() {
            if (_reader != null)
                _reader.Tag -= OnTag;
        }

        ~LinkBuilder() {
            Dispose();
        }
    }
}