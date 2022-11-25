namespace HaDocument.Models {
    public class Tradition {
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