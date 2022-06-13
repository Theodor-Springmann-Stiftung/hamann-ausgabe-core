namespace HaDocument.Models;
using System.Xml.Linq;

public class Editreason {
    public string Index { get; }
    public XElement Element { get; }
    public string Value { get; }
    public string Letter { get; }
    
    public Editreason(
        string index,
        XElement element,
        string value,
        string letter
    ) {
        Index = index;
        Value = value;
        Element = element;
        Letter = letter;
    }
}