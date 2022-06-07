namespace HaDocument.Models {
    public class Letter : HaModel {
        public string Index { get; } = "";
        public string Element { get; } = "";
        public string Value { get; } = "";

        public Letter(
            string index,
            string element,
            string value
        ) {
            Index = index;
            Element = element;
        }
    }
}