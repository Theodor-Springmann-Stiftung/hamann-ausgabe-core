namespace HaWeb.XMLTests;
using HaWeb.Models;
using System.Xml.Linq;

public interface ICollectionRule {
    public string Name { get; }
    public HamannXPath[] Bases { get; }
    public HamannXPath[] Backlinks { get; }
    public IEnumerable<(string, XElement, XMLRootDocument)> GenerateIdentificationStrings(IEnumerable<(XElement, XMLRootDocument)> List);
    public IEnumerable<(string, XElement, XMLRootDocument, bool)> GenerateBacklinkString(IEnumerable<(XElement, XMLRootDocument)> List);
    public bool CheckDatatypes(XElement element);
}