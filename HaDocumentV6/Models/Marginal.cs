namespace HaDocument.Models {
    public class Marginal : HaDocument.Interfaces.ISearchable {
        public string Index { get; } = "";
        public string Letter { get; } = "";
        public string Page { get; } = "";
        public string Line { get; } = "";
        public string Element { get; } = "";
        
        public Marginal(
            string index,
            string letter,
            string page,
            string line,
            string element
        ) {
            Index = index;
            Letter = letter;
            Page = page;
            Line = line;
            Element = element;
        }
    }
}