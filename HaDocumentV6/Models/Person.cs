namespace HaDocument.Models {
    public class Person {
        public string Index { get; } = "";
        public string Name { get; } = "";
        public string Prename { get; } = "";
        public string Surname { get; } = "";
        public string? Komm { get; }

        public Person(
            string index,
            string name,
            string prename,
            string surname,
            string? komm
        ) {
            Index = index;
            Name = name;
            Prename = prename;
            Surname = surname;
            Komm = komm;
        }
    }
}