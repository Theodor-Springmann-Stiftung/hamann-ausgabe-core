using System.Xml.Linq;

namespace HaDocument.Models {
    public class Letter {
        public string ID { get; } = "";
        public string Element { get; } = "";
        public XElement? XElement { get; }

        public Letter(
            string id,
            string element,
            XElement? xelement = null
        ) {
            ID = id;
            Element = element;
            XElement = xelement;
        }

        public static Letter? FromXElement(XElement element) {
            if (!element.HasAttributes || element.IsEmpty || element.Name != "letterText") return null;
            if (element.Attribute("letter")?.Value == null) return null;
            return new Letter(
                element.Attribute("letter")!.Value,
                element.ToString(),
                element
            );
        }
    }
}