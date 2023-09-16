using System.Xml.Linq;

namespace HaDocument.Models {
    public class Location {
        public string Index { get; } = "";
        public string Name { get; } = "";
        public string? Reference { get; }
        public XElement? XElement { get; }
        
        public Location(
            string index, 
            string name,
            string? reference,
            XElement? xelement = null
        ) {
            Index = index;
            Name = name;
            XElement = xelement;
            Reference = reference;
        }

        public static Location? FromXElement(XElement element) {
            if (!element.HasAttributes || element.Name != "locationDef") return null;
            if (element.Attribute("index")?.Value == null || element.Attribute("name")?.Value == null) return null;
            return new Location(
                element.Attribute("index")!.Value,
                element.Attribute("name")!.Value,
                element.Attribute("ref")?.Value,
                element
            );
        }
    }
}