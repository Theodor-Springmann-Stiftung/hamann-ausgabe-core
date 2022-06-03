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
            if (instance != null) this._Roots.Add(instance.Prefix, instance);
        });
    }

    public IXMLRoot? GetRoot(string name) {
        _Roots.TryGetValue(name, out var root);
        return root;
    }

    public List<IXMLRoot>? GetRoots() => this._Roots == null ? null : this._Roots.Values.ToList();

    public List<XMLRootDocument>? ProbeHamannFile(XDocument document, ModelStateDictionary ModelState) {
        if (document.Root!.Name != "opus") {
            ModelState.AddModelError("Error", "A valid Hamann-Docuemnt must begin with <opus>");
            return null;
        }

        List<XMLRootDocument>? res = null;
        if (document.Root != null && _Roots != null) {
            foreach (var (_, root) in _Roots) {
                var elements = root.IsTypeOf(document.Root);
                if (elements != null &&  elements.Any())
                    foreach (var elem in elements) {
                        if (res == null) res = new List<XMLRootDocument>();
                        res.Add(_createXMLRootDocument(root, elem));
                    }
            }
        }
        if (res == null) ModelState.AddModelError("Error", "Kein zum Hamann-Briefe-Projekt passendes XML gefunden.");
        return res;
    }

    private XMLRootDocument _createXMLRootDocument(IXMLRoot Root, XElement element) {
        var doc = new XMLRootDocument(this, Root.Prefix, Root.GenerateIdentificationString(element), element);
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