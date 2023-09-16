using System.Xml.Linq;

namespace HaDocument.Models {
    public class Tradition {
        public string ID { get; } = "";
        public string Element { get; } = "";

        public XElement? XElement { get; }

        public Tradition(
            string id,
            string element,
            XElement? xelement = null
        ) {
            ID = id;
            Element = element;
            XElement = xelement;
        }

        public static Tradition? FromXElement(XElement element) {
            if (!element.HasAttributes || element.IsEmpty || element.Name != "letterTradition") return null;
            if (element.Attribute("letter")?.Value == null) return null;
            return new Tradition(
                element.Attribute("letter")!.Value,
                element.ToString(),
                element
            );
        }
    }
}