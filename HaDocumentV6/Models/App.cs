namespace HaDocument.Models {
    public class App {
        public string Index { get; } = "";
        public string Name { get; } = "";
        public bool Category { get; } = false;

        public App(
            string index, 
            string name,
            bool category
        ) {
            Index = index;
            Name = name;
        }
    }
}