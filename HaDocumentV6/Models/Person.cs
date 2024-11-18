using System.Xml.Linq;

namespace HaDocument.Models {
    public class Person {
        public string Index { get; } = string.Empty;
        public string Name { get; } = string.Empty;
        public string? Prename { get; }
        public string? Surname { get; }
        public string? Komm { get; }
        public string? Reference { get; }
        public bool IsOrg { get; } = false;
        public XElement? XElement { get; }

        public Person(
            string index,
            string name,
            string? prename,
            string? surname,
            string? komm,
            string? reference,
            XElement? xElement = null,
            bool IsOrg = false
        ) {
            Index = index;
            Name = name;
            Prename = prename;
            Surname = surname;
            Komm = komm;
            Reference = reference;
            this.XElement = xElement;
            this.IsOrg = IsOrg;
        }

        public static Person? FromXElement(XElement element) {
            if (!element.HasAttributes || (element.Name != "personDef" && element.Name != "handDef")) return null;
            if (element.Attribute("index")?.Value == null || element.Attribute("name")?.Value == null) return null;
            var org = element.HasAttributes && element.Attribute("org")?.Value == "true";
            return new Person(
                element.Attribute("index")!.Value,
                element.Attribute("name")!.Value,
                element.Attribute("vorname")?.Value,
                element.Attribute("nachname")?.Value,
                element.Attribute("komm")?.Value,
                element.Attribute("ref")?.Value,
                element,
                org
            );
        }
    }
}
