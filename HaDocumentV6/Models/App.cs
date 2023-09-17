using System.Xml.Linq;

namespace HaDocument.Models {
    public class App : IHaElement {
        public string ElementName { get; } = "appDef";
        public string[] XPath { get; } = {
            "/opus/data/definitions/appDefs/appDef",
            "/opus/definitions/appDefs/appDef"
        };
        public string ElementRules { get; } = "Pfad: /opus/definitions/appDefs. Pflicht-Attribute: index (einmalig), name, category.";
        public bool Searchable { get; } = false;
        public XElement? XElement { get; }

        public string Index { get; } = "";
        public string Name { get; } = "";
        public string Category { get; } = "";

        public App(
            string index, 
            string name,
            string category,
            XElement? xElement = null
        ) {
            Index = index;
            Name = name;
            Category = category;
            XElement = xElement;
        }

        public static App? FromXElement(XElement element) {
            if (!element.HasAttributes || element.Name != "appDef") return null;
            if (element.Attribute("index")?.Value == null || element.Attribute("name")?.Value == null || element.Attribute("category")?.Value == null) return null;
            return new App(
                element.Attribute("index")!.Value,
                element.Attribute("name")!.Value,
                element.Attribute("category")!.Value,
                element
            );        
        }

        public string GetKey() => this.Index;

        public int CompareTo(object? obj) {
            if (obj == null) return 1;
            var other = (App)obj;
            if (Int32.TryParse(Index, out var thisindex) && Int32.TryParse(other.Index, out var otherindex) )
                return thisindex.CompareTo(otherindex);
            return String.Compare(this.Index, other.Index);
        }
    }
}