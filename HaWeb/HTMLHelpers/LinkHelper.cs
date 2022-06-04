namespace HaWeb.HTMLHelpers;
using HaDocument.Interfaces;
using HaDocument.Models;
using System;
using System.Text;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using HaXMLReader;
using System.Collections.Generic;
using HaWeb.Settings.ParsingRules;
using HaWeb.Settings.ParsingState;

public class LinkHelper {
    private ILibrary _lib;
    private IReader _reader;
    private StringBuilder _sb;

    private bool _followlinksinchildren;
    private bool _followlinksinthis;

    private static readonly string DEFAULTELEMENT = HaWeb.Settings.HTML.DEFAULTELEMENT;
    private static readonly string LETLINKCLASS = HaWeb.Settings.CSSClasses.LETLINKCLASS;
    private static readonly string REFLINKCLASS = HaWeb.Settings.CSSClasses.REFLINKCLASS;
    private static readonly string WWWLINKCLASS = HaWeb.Settings.CSSClasses.WWWLINKCLASS;

    public LinkHelper(ILibrary lib, IReader reader, StringBuilder stringBuilder, bool followlinksinchildren = true, bool followlinksinthis = true) {
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
                _sb.Append(HTMLHelpers.TagHelpers.CreateEndElement("a"));
            } else {
                if (tag.Name == "wwwlink" && tag.Values.ContainsKey("address") && _followlinksinthis)
                    _sb.Append(HTMLHelpers.TagHelpers.CreateCustomElement("a",
                        new HaWeb.HTMLHelpers.TagHelpers.Attribute() { Name = "class", Value = WWWLINKCLASS },
                        new HaWeb.HTMLHelpers.TagHelpers.Attribute() { Name = "href", Value = tag["address"] },
                        new HaWeb.HTMLHelpers.TagHelpers.Attribute() { Name = "target", Value = "_blank" },
                        new HaWeb.HTMLHelpers.TagHelpers.Attribute() { Name = "rel", Value = "noopener noreferrer" }));
                if (tag.Name == "intlink" && tag.Values.ContainsKey("letter") && _lib.Metas.ContainsKey(tag["letter"])) {
                    var letter = _lib.Metas[tag["letter"]];
                    _sb.Append(HTMLHelpers.TagHelpers.CreateElement("a", LETLINKCLASS, "/Briefe/" + letter.Autopsic + "#" + tag["page"] + "-" + tag["line"]));
                    if (!tag.Values.ContainsKey("linktext") || tag.Values["linktext"] == "true") {
                        var linkstring = "";
                        var ZHstring = "";
                        var pglnstring = "";
                        linkstring += "HKB&nbsp;" + letter.Autopsic;
                        if (tag.Values.ContainsKey("page")) {
                            pglnstring += tag["page"];
                            if (tag.Values.ContainsKey("line")) {
                                pglnstring += "/" + tag["line"];
                            }
                            if (letter.ZH != null)
                                ZHstring += HTMLHelpers.ConversionHelpers.ToRoman(Int32.Parse(letter.ZH.Volume)) + "&nbsp;";
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
                                _sb.Append(HTMLHelpers.TagHelpers.CreateElement("a", REFLINKCLASS, "/Register/Register/" + linkloc[0] + "#" + comment.Index));
                            else if (comment.Type == "bibel")
                                _sb.Append(HTMLHelpers.TagHelpers.CreateElement("a", REFLINKCLASS, "/Register/Bibelstellen/" + linkloc[0] + linkloc[1] + "#" + comment.Index));
                            else if (comment.Type == "forschung")
                                _sb.Append(HTMLHelpers.TagHelpers.CreateElement("a", REFLINKCLASS, "/Register/Forschung/" + linkloc[0] + "#" + comment.Index));
                        _sb.Append(GetLemmaString(tag, comment));
                    }
                }
                if (tag.IsEmpty && _followlinksinthis) _sb.Append(HTMLHelpers.TagHelpers.CreateEndElement("a"));
            }
        }
    }

    private string GetLemmaString(Tag tag, Comment comment) {
        if (!tag.Values.ContainsKey("linktext") || tag.Values["linktext"] == "true") {
            var linkState = new LinkState();
            var sb = new StringBuilder();
            var subreader = new UTF8StringReader(comment.Lemma);
            new LinkHelper(_lib, subreader, sb, _followlinksinchildren, _followlinksinchildren);
            new HTMLParser.XMLHelper<LinkState>(linkState, subreader, sb, LinkRules.OTagRules, null, LinkRules.CTagRules, LinkRules.TextRules, null);
            subreader.Read();
            return sb.ToString();
        }
        return "";
    }

    public void Dispose() {
        if (_reader != null)
            _reader.Tag -= OnTag;
    }

    ~LinkHelper() {
        Dispose();
    }
}