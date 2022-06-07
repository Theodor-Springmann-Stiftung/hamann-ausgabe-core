namespace HaDocument.Models {
    public class Backlink {
        public string Index { get; } = "";

        public string Letter { get; } = "";
        public string Page { get; } = "";
        public string Line { get; } = "";
        public string MarginalIndex { get; } = "";

        public Backlink(
            string index,
            string letter,
            string page,
            string line,
            string marginalindex
        ) {
            Index = index;
            Letter = letter;
            Page = page;
            Line = line;
            MarginalIndex = marginalindex;
        }
    }
}