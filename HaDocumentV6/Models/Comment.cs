using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Linq;

namespace HaDocument.Models {
    public class Comment : IHaElement {
        public string ElementName { get; } = "link";
        public string[] XPath { get; } = { 
            "/opus/kommentare/kommentar/kommcat/kommentar", 
            "/opus/data/kommentare/kommentar/kommcat/kommentar",
        };
        public string ElementRules { get; } = "Pfad: /opus/kommentare/kommentar. Pflicht-Attribute: id (einmalig).";
        public bool Searchable { get; } = true;
        public XElement? XElement { get; }

        public string Element { get; } = "";
        public string Index { get; } = "";
        public string Type { get; } = "";
        public string Lemma { get; } = "";
        public string Parent { get; } = "";
        public int? Order { get; } = null;
        public ImmutableSortedDictionary<string, Comment>? Kommentare { get; } 

        public Comment(
            string entry,
            string index,
            string? type,
            string? lemma,
            int? order,
            ImmutableSortedDictionary<string, Comment>? subComments,
            string? parent = null,
            XElement? xelement = null
        ) {
            Element = entry;
            Index = index;
            Type = type;
            Lemma = lemma;
            Order = order;
            Parent = parent;
            XElement = xelement;
            Kommentare = subComments;
        }

        public String GetKey() => Index;

        public int CompareTo(object? obj) {
            if (obj == null) return 1;
            var other = (Comment)obj;
            if (!String.IsNullOrWhiteSpace(Parent) && !String.IsNullOrWhiteSpace(other.Parent) &&
                (Parent == other.Parent)) {
                    if (Order.HasValue && other.Order.HasValue)
                        return Order.Value!.CompareTo(other.Order.Value);
                    else if (Order.HasValue)
                        return 1;
                    else if (other.Order.HasValue)
                        return -1;
                    else
                        return 0;
            }
            return String.Compare(Index, other.Index);
        }

        public Comment? FromXElement(XElement? element) {
            if (element == null || !element.HasAttributes || element.IsEmpty) return null;
            if (element.Attribute("id")?.Value == null) return null;
            var cat = element.Ancestors("kommcat");
            if (element.Name == "kommentar")
                return new Comment(
                    element.ToString(),
                    element.Attribute("id")!.Value,
                    (cat.Any() ? cat.First().Attribute("value")?.Value : null) ?? element.Attribute("type")?.Value,
                    element.Element("lemma")?.Value,
                    element.Attribute("sort")?.Value != null ? (Int32.TryParse(element.Attribute("sort")!.Value, out var s) ? s : null) : null,
                    element.Elements("subsection").Any() ? element.Elements("subsection").Select(x => FromXElement(x)).ToImmutableSortedDictionary(x => x.Index, y => y) : null,
                    null,
                    element
                );
            else if (element.Name == "subsection") {
                if (element.Ancestors("kommentar").Any() || element.Ancestors("kommentar")!.First().Attribute("id")?.Value == null) return null;
                return new Comment(
                    element.ToString(),
                    element.Attribute("id")!.Value,
                     (cat.Any() ? cat.First().Attribute("value")?.Value : null) ?? element.Attribute("type")?.Value,
                    element.Element("lemma")?.Value,
                    element.Attribute("sort")?.Value != null ? (Int32.TryParse(element.Attribute("sort")!.Value, out var s) ? s : null) : null,
                    null,
                    element.Ancestors("kommentar")!.First().Attribute("id")!.Value,
                    element
                );
            }
            return null;
        }
    }
}