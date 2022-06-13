namespace HaDocument.Models;
using System.Xml.Linq;
using System.Xml;

public class Letter {
    public string Index { get; }
    public XElement Element { get; }
    public string Value { get; }

    public Letter(
        string index,
        XElement element,
        string value
    ) {
        Index = index;
        Element = element;
        Value = value;
    }
}