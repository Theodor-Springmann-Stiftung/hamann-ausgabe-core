namespace HaWeb.XMLTests;
using HaWeb.Models;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

public class XMLTester {
    private Dictionary<string, INodeRule>? _Ruleset;
    private List<XMLRootDocument>? _Documents;
    private Dictionary<string, HashSet<string>>? _IDs;
    private Dictionary<string, List<(XElement, XMLRootDocument)>?> _XPathEvaluated;
    public XMLTester (IXMLTestService testService, Dictionary<string, Models.FileList?>? filelists) {
        _Ruleset  = testService.Ruleset;
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

    public void Test() {
        if (_Ruleset == null) return;        
        _IDs = new System.Collections.Generic.Dictionary<string, HashSet<string>>();
        foreach (var rule in _Ruleset) {
            buildIDs(rule.Value);
            checkRequiredAttributes(rule.Value);
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
            checkUniqueness(rule.Name, rule.XPath, rule.uniquenessAttribute);
        }
        if (rule.References != null && rule.References.Any()) {
            foreach (var reference in rule.References) {
                checkUniqueness(rule.Name, reference.RemoteElement, reference.RemoteAttribute);
            }
        }
    }

    private void checkUniqueness(string name, string xpathelement, string attribute) {
        if (_Documents == null || _IDs == null || _IDs.ContainsKey(name)) return;
        var hs = new HashSet<string>();
        var elements = GetEvaluateXPath(xpathelement);
        if (elements != null)
            foreach (var e in elements) {
                if (checkAttribute(e.Item1, attribute, e.Item2)) {
                    if (!hs.Add(e.Item1.Attribute(attribute)!.Value)) {
                        e.Item2.Log(generateLogMessage(e.Item1) + "Attributwert " + e.Item1.Attribute(attribute)!.Value + " doppelt.");
                    }
                }
            }
            _IDs.TryAdd(name, hs);
    }

    private bool checkAttribute(XElement element, string attributename, XMLRootDocument doc) {
        if (!element.HasAttributes || element.Attribute(attributename) == null) {
            doc.Log(generateLogMessage(element) + "Attribut " + attributename + " fehlt.");
            return false;
        }
        return true;
    }

    private string generateLogMessage(XElement element) {
        return "Zeile " + 
            ((IXmlLineInfo)element).LineNumber.ToString() + 
            ", Element " +
            element.Name +
            ": ";
    }

    private List<(XElement, XMLRootDocument)>? GetEvaluateXPath(string xpath) {
        if (_XPathEvaluated.ContainsKey(xpath)) return _XPathEvaluated[xpath];
        if (!_XPathEvaluated.ContainsKey(xpath)) _XPathEvaluated.Add(xpath, null);
        if (_Documents == null) return null;
        foreach (var d in _Documents) {
            var elements = d.GetElement().XPathSelectElements(xpath).ToList();
            if (elements != null && elements.Any()) {
                if (_XPathEvaluated[xpath] == null) _XPathEvaluated[xpath] = new List<(XElement, XMLRootDocument)>();
                foreach (var res in elements) {
                    _XPathEvaluated[xpath]!.Add((res, d));
                }
            }
        }
        return _XPathEvaluated[xpath];
    }
}