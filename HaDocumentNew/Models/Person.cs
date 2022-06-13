namespace HaDocument.Models;

public class Person {
    public string Index { get; }
    public string Name { get; }
    public string? Prename { get; }
    public string? Surname { get; }

    public Person(
        string index,
        string name,
        string? prename,
        string? surname
    ) {
        Index = index;
        Name = name;
        Prename = prename;
        Surname = surname;
    }
}
