namespace HaDocument.Models {
    public class Tradition : HaDocument.Interfaces.ISearchable {
        public string Index { get; } = "";
        public string Element { get; } = "";

        public Tradition(
            string index,
            string element
        ) {
            Index = index;
            Element = element;
        }
    }
}