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
            string elemnt
        ) {
            Index = index;
            Letter = letter;
            Page = page;
            Line = line;
            Element = elemnt;
        }
    }
}