namespace HaDocument.Models {
    public class Letter : HaModel {
        public string Index { get; } = "";
        public string Element { get; } = "";

        public Letter(
            string index,
            string element
        ) {
            Index = index;
            Element = element;
        }
    }
}