namespace HaDocument.Models {
    public class App {
        public string Index { get; } = "";
        public string Name { get; } = "";
        public string Category { get; } = "";

        public App(
            string index, 
            string name,
            string category
        ) {
            Index = index;
            Name = name;
            Category = category;
        }
    }
}