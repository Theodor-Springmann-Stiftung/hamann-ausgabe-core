namespace HaDocument.Models {
    public class Location {
        public string Index { get; } = "";
        public string Name { get; } = "";

        public Location(
            string index, 
            string name
        ) {
            Index = index;
            Name = name;
        }
    }
}