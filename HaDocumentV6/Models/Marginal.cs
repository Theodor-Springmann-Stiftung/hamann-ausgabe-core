using System.Xml.Linq;

namespace HaDocument.Models {
    public class Marginal {
        public string? Sort { get; } = "";
        public string Letter { get; } = "";
        public string Page { get; } = "";
        public string Line { get; } = "";
        public string Element { get; } = "";
        public XElement? XElement { get; }

        public Marginal(
            string letter,
            string page,
            string line,
            string? sort,
            string element,
            XElement? xelement = null
        ) {
            Letter = letter;
            Page = page;
            Line = line;
            Sort = sort;
            Element = element;
            XElement = xelement;
        }

        public static Marginal? FromXElement(XElement element) {
            if (!element.HasAttributes || element.Name != "marginal" || element.IsEmpty) return null;
            if (element.Attribute("letter")?.Value == null || element.Attribute("page")?.Value == null || element.Attribute("line")?.Value == null) return null;
            return new Marginal(
                element.Attribute("letter")!.Value, 
                element.Attribute("page")!.Value, 
                element.Attribute("line")!.Value,
                element.Attribute("sort")?.Value,
                element.ToString(),
                element
            );
        }
    }
}