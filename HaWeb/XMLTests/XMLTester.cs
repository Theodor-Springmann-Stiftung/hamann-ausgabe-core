namespace HaWeb.XMLTests;
using HaWeb.Models;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

public class XMLTester {
    private Dictionary<string, INodeRule>? _Ruleset;
    private Dictionary<string, ICollectionRule>? _CollectionRuleset;
    private List<XMLRootDocument>? _Documents;
    private Dictionary<string, SyntaxCheckModel> _Results;
    private Dictionary<string, HashSet<string>>? _IDs;
    private Dictionary<string, HashSet<string>>? _CollectionIDs;
    private Dictionary<string, List<(XElement, XMLRootDocument)>?> _XPathEvaluated;
    public XMLTester (IXMLTestService testService, Dictionary<string, Models.FileList?>? filelists, Dictionary<string, SyntaxCheckModel> results) {
        _Ruleset  = testService.Ruleset;
        _CollectionRuleset = testService.CollectionRuleset;
        _Results = results;
        if (filelists != null) {
            foreach (var fl in filelists) {
                if (fl.Value != null)  {
                    if (_Documents == null) _Documents = new List<XMLRootDocument>();
                    var docs = fl.Value.GetFileList();
                    if (docs != null) _Documents.AddRange(docs);
                }
            }
        }
        _XPathEvaluated = new Dictionary<string, List<(XElement, XMLRootDocument)>?>();
    }

    public Dictionary<string, SyntaxCheckModel>? Test() {
        if (_Ruleset == null) return null;        
        _IDs = new Dictionary<string, HashSet<string>>();
        foreach (var rule in _Ruleset) {
            buildIDs(rule.Value);
            checkRequiredAttributes(rule.Value);
            checkReferences(rule.Value);
        }
        if (_CollectionRuleset == null) return null;
        _CollectionIDs = new Dictionary<string, HashSet<string>>();
        foreach (var collectionrule in _CollectionRuleset) {
            buildIDs(collectionrule.Value);
            checkReferences(collectionrule.Value);
        }
        return _Results;
    }

    private void checkReferences(INodeRule rule) {
        if (rule.References == null || !rule.References.Any()) return;
        var elements = GetEvaluateXPath(rule.XPath);
        if (elements != null && elements.Any()) {
            foreach (var e in elements) {
                foreach (var r in rule.References) {
                    var hasattr = checkAttribute(e.Item1, r.LinkAttribute, e.Item2, false);
                    var keyname = r.RemoteElement.XPath + "-" + r.RemoteAttribute;
                    if (_IDs != null && _IDs.ContainsKey(keyname) && hasattr) {
                        var val = e.Item1.Attribute(r.LinkAttribute)!.Value;
                        if (!_IDs[keyname].Contains(val)) {
                            var lc = getLineColumn(e.Item1);
                            _Results[e.Item2.File.FileName].Log(lc.Item1, lc.Item2, "Verlinktes Element " + val + " nicht gefunden.");
                        }
                    }
                }
            }
        }
    }

    private void checkReferences(ICollectionRule rule) {
        if (rule.Backlinks == null || !rule.Backlinks.Any() || !_CollectionIDs.ContainsKey(rule.Name)) return;
        foreach (var bl in rule.Backlinks) {
            var elemens = GetEvaluateXPath(bl);
            if (elemens != null && elemens.Any()) {
                foreach(var r in rule.GenerateBacklinkString(elemens)) {
                    if (!r.Item4 && !_CollectionIDs[rule.Name].Contains(r.Item1)) {
                        var lc = getLineColumn(r.Item2);
                        _Results[r.Item3.File.FileName].Log(lc.Item1, lc.Item2, "Verlinktes Element " + r.Item1 + " nicht gefunden.");
                    }
                    if (r.Item4) {
                        var coll = _CollectionIDs[rule.Name];
                        var items = r.Item1.Split('-');
                        var searchterm = items[0];
                        var found = coll.Where(x => x.StartsWith(searchterm));
                        if (items[0] == "NA" || found == null || !found.Any()) {
                            var lc = getLineColumn(r.Item2);
                            _Results[r.Item3.File.FileName].Log(lc.Item1, lc.Item2, "Verlinktes Element " + r.Item1 + " nicht gefunden.");
                        } else {
                            for (var i = 1; i < items.Length; i++) {
                                if (items[i] == "NA") break;
                                else {
                                    searchterm = searchterm + "-" + items[i];
                                    found = found.Where(x => x.StartsWith(searchterm));
                                    if (found == null || !found.Any()) {
                                        var lc = getLineColumn(r.Item2);
                                        _Results[r.Item3.File.FileName].Log(lc.Item1, lc.Item2, "Verlinktes Element " + r.Item1 + " nicht gefunden.");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void checkRequiredAttributes(INodeRule rule) {
        if (rule.Attributes == null) return;
        var elements = GetEvaluateXPath(rule.XPath);
        if (elements != null && elements.Any()) {
            foreach (var e in elements) {
                foreach (var attr in rule.Attributes) {
                    checkAttribute(e.Item1, attr, e.Item2);
                }
            }
        }
    }

    private void buildIDs(INodeRule rule) {
        if (!String.IsNullOrWhiteSpace(rule.uniquenessAttribute)) {
            checkUniqueness(rule.XPath, rule.uniquenessAttribute);
        }
        if (rule.References != null && rule.References.Any()) {
            foreach (var reference in rule.References) {
                checkUniqueness(reference.RemoteElement, reference.RemoteAttribute);
            }
        }
    }

    private void buildIDs(ICollectionRule rule) {
        if (rule.Bases != null && rule.Bases.Any()) {
            var hs = new HashSet<string>();
            foreach (var b in rule.Bases) {
                var elemens = GetEvaluateXPath(b);
                if (elemens != null && elemens.Any()) {
                    foreach (var r in rule.GenerateIdentificationStrings(elemens)) {
                        if (!hs.Add(r.Item1)) {
                            var lc = getLineColumn(r.Item2);
                            _Results[r.Item3.File.FileName].Log(lc.Item1, lc.Item2, "Identifikator (Brief-Seite-Zeile(-Sort))" + r.Item1 + " mehrdeutig.");
                        }
                    }
                    foreach (var e in elemens) {
                        if (!rule.CheckDatatypes(e.Item1)) {
                            var lc = getLineColumn(e.Item1);
                            _Results[r.Item3.File.FileName].Log(lc.Item1, lc.Item2, "Attributwert: Datentyp nicht zugelassen.");
                        }
                    }
                }
            }
            _CollectionIDs!.TryAdd(rule.Name, hs);
        }
    }

    private void checkUniqueness(HamannXPath xpathelement, string attribute) {
        if (_Documents == null || _IDs == null || _IDs.ContainsKey(xpathelement.XPath + "-" + attribute)) return;
        var hs = new HashSet<string>();
        var elements = GetEvaluateXPath(xpathelement);
        if (elements != null)
            foreach (var e in elements) {
                if (checkAttribute(e.Item1, attribute, e.Item2)) {
                    if (!hs.Add(e.Item1.Attribute(attribute)!.Value)) {
                        var lc = getLineColumn(e.Item1);
                        _Results[e.Item2.File.FileName].Log(lc.Item1, lc.Item2, "Attributwert " + e.Item1.Attribute(attribute)!.Value + " doppelt.");
                    }
                }
            }
            _IDs.TryAdd(xpathelement.XPath  + "-" + attribute, hs);
    }

    private bool checkAttribute(XElement element, string attributename, XMLRootDocument doc, bool log = true) {
        if (!element.HasAttributes || element.Attribute(attributename) == null) {
            if (log) {
                var lc = getLineColumn(element);
                _Results[doc.File.FileName].Log(lc.Item1, lc.Item2,"Attribut " + attributename + " fehlt.");
            };
            return false;
        }
        return true;
    }

    private (int, int) getLineColumn(XElement element) {
        return (((IXmlLineInfo)element).LineNumber, ((IXmlLineInfo)element).LinePosition);
    }

    // Cache for XPATH evaluation
    private List<(XElement, XMLRootDocument)>? GetEvaluateXPath(HamannXPath xpath) {
        if (_Documents == null || xpath == null) return null;
        if (_XPathEvaluated.ContainsKey(xpath.XPath)) return _XPathEvaluated[xpath.XPath];
        if (!_XPathEvaluated.ContainsKey(xpath.XPath)) _XPathEvaluated.Add(xpath.XPath, null);
        foreach (var d in _Documents) {
            if (xpath.Documents != null && !xpath.Documents.Contains(d.Prefix)) continue;
            var elements = d.Element.XPathSelectElements("." + xpath.XPath).ToList();
            if (elements != null && elements.Any()) {
                if (_XPathEvaluated[xpath.XPath] == null) _XPathEvaluated[xpath.XPath] = new List<(XElement, XMLRootDocument)>();
                foreach (var res in elements) {
                    _XPathEvaluated[xpath.XPath]!.Add((res, d));
                }
            }
        }
        return _XPathEvaluated[xpath.XPath];
    }
}