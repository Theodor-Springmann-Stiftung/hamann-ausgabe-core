using System.Xml.Linq;

namespace HaDocument.Models {
    public class Backlink {
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
            string? comment = null
        ) {
            Href = href;
            Letter = letter;
            Page = page;
            Line = line;
            Comment = comment;
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
                    marginal.Attribute("line")!.Value
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
                    subsection.Attribute("id")!.Value
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
                    kommentar.Attribute("id")!.Value
                );
            }

            return null;
        }
    }
}