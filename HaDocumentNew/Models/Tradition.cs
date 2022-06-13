namespace HaDocument.Models;
using System.Xml.Linq;

public class Tradition {
    public string Index { get; }
    public XElement Element { get; }
    public string Value;

    public Tradition(
        string index,
        XElement element,
        string value
    ) {
        Index = index;
        Element = element;
        Value = value;
    }
}