using System.Xml.Linq;

namespace HaDocument.Models {
    public class App {
        public string Index { get; } = "";
        public string Name { get; } = "";
        public string Category { get; } = "";

        public XElement? XElement { get; }

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
    }
}