using System.Xml.Linq;

namespace HaDocument.Models {
    public class Backlink : IHaElement {
        public string ElementName { get; } = "link";
        public string[] XPath { get; } = { 
            "/opus/data/marginalien/marginal//link", 
            "/opus/marginalien/marginal//link", 
            "/opus/kommentare/kommentar//link", 
            "/opus/data/kommentare/kommentar//link",
            "/opus/traditions/letterTradition//link", 
            "/opus/data/traditions/letterTradition//link",
        };
        public string ElementRules { get; } = "Pfad: /opus/marginalien, /opus/kommentare, /opus/traditions. Pflicht-Attribute: letter.";
        public bool Searchable { get; } = false;
        public XElement? XElement { get; } = null;

        public string Href { get; } = "";
        public string? Letter { get; } = "";
        public string? Page { get; } = "";
        public string? Line { get; } = "";
        public string? Comment { get; }

        public Backlink(
            string href,
            string? letter,
            string? page,
            string? line,
            string? comment = null,
            XElement? xElement = null
        ) {
            Href = href;
            Letter = letter;
            Page = page;
            Line = line;
            Comment = comment;
            XElement = xElement;
        }

        public static Backlink? FromXElement(XElement element) { 
            if (!element.HasAttributes || element.Name != "link") return null;
            if (element.Attribute("ref")?.Value == null && element.Attribute("subref")?.Value == null) return null;
            
            if (element.Ancestors("marginal") == null || !element.Ancestors("marginal").Any()) {
                var marginal = element.Ancestors("marginal").First();
                if (Marginal.FromXElement(marginal) == null) return null;
                return new Backlink(
                    element.Attribute("subref")?.Value ?? element.Attribute("ref")!.Value,
                    marginal.Attribute("letter")!.Value,
                    marginal.Attribute("page")!.Value,
                    marginal.Attribute("line")!.Value,
                    null,
                    element
                );
            }

            if (element.Ancestors("subsection") != null || !element.Ancestors("subsection").Any()) {
                var subsection = element.Ancestors("subsection").First();
                if (subsection.Attribute("id")?.Value == null) return null;
                return new Backlink(
                    element.Attribute("subref")?.Value ?? element.Attribute("ref")!.Value,
                    null,
                    null,
                    null,
                    subsection.Attribute("id")!.Value,
                    element
                );
            }

            if (element.Ancestors("kommentar") != null || !element.Ancestors("kommentar").Any()) {
                var kommentar = element.Ancestors("kommentar").First();
                if (kommentar.Attribute("id")?.Value == null) return null;
                return new Backlink(
                    element.Attribute("subref")?.Value ?? element.Attribute("ref")!.Value,
                    null,
                    null,
                    null,
                    kommentar.Attribute("id")!.Value,
                    element
                );
            }

            return null;
        }

        public string GetKey() {
            return string.Empty;
        }

        public int CompareTo(object? obj) {
            return 0;
        }
    }
}