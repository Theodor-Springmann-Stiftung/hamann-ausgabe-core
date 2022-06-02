namespace HaWeb.XMLParser;
using HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class XMLService : IXMLService {
    private Dictionary<string, IXMLRoot>? _Roots;

    public XMLService() {
        var types = _GetAllTypesThatImplementInterface<IXMLRoot>().ToList();
        types.ForEach( x => {
            if (this._Roots == null) this._Roots = new Dictionary<string, IXMLRoot>();
            var instance = (IXMLRoot)Activator.CreateInstance(x)!;
            if (instance != null) this._Roots.Add(instance.Type, instance);
        });
    }

    public List<IXMLRoot>? GetRoots() => this._Roots == null ? null : this._Roots.Values.ToList();

    public List<XMLRootDocument>? ProbeHamannFile(XDocument document, ModelStateDictionary ModelState) {
        if (document.Root!.Name != "opus") {
            ModelState.AddModelError("Error", "A valid Hamann-Docuemnt must begin with <opus>");
            return null;
        }

        var res = _testElements(document.Root.Elements());
        if (document.Root.Element("data") != null) {
            var datares = _testElements(document.Element("data")!.Elements());
            if (datares != null && res == null) res = datares;
            else if (datares != null) res!.AddRange(datares);
        }

        return res;
    }

    private List<XMLRootDocument>? _testElements(IEnumerable<XElement>? elements) {
        if (elements == null) return null;
        List<XMLRootDocument>? res = null;
        foreach (var elem in elements) {
           var doc = _testElement(elem);
           if (doc != null) {
               if (res == null) res = new List<XMLRootDocument>();
               res.Add(doc);
           }
        }
        return res;
    }

    private XMLRootDocument? _testElement(XElement? element) {
        if (element == null || _Roots == null) return null;
        foreach (var (_, root) in _Roots) {
            if(root.IsTypeOf(element))
                return _createXMLRootDocument(root, element);
        }
        return null;
    }

    private XMLRootDocument _createXMLRootDocument(IXMLRoot Root, XElement element) {
        var doc = new XMLRootDocument(Root.Type, Root.GenerateIdentificationString(element), element);
        doc.Elements = Root.GetCollectedObjects(doc);
        doc.Fields = Root.GenerateFields(doc);
        return doc;
    }

    private IEnumerable<Type> _GetAllTypesThatImplementInterface<T>()
    {
        return System.Reflection.Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
    }

}