namespace HaDocument.Models;
using System.Xml.Linq;

public class Marginal {
    public string Index { get; }
    public string Letter { get; }
    public string Page { get; }
    public string Line { get; }
    public XElement Element { get; }
    public string Value { get; }

    public Marginal(
        XElement element,
        string value,
        string index,
        string letter,
        string page,
        string line
    ) {
        Index = index;
        Letter = letter;
        Page = page;
        Line = line;
        Element = element;
        Value = value;
    }
}
