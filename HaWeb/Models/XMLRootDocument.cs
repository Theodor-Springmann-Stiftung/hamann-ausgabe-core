namespace HaWeb.Models;
using System.Xml.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.FileProviders;
using HaWeb.XMLParser;
using System.Text;

public class XMLRootDocument {
    [JsonIgnore]
    public XElement? Element { get; private set; }
    [JsonIgnore]
    public IXMLRoot XMLRoot { get; private set; }
    public FileModel File { get; private set; }

    public string Prefix { get; private set; }
    // UNUSED AS OF NOW
    public (string?, string?) IdentificationString { get; private set; }
    // TODO: Fields
    public List<(string, string?)>? Fields { get; set; }

    // Entry point for XML file reading
    public XMLRootDocument(IXMLRoot xmlRoot, string prefix, (string?, string?) idString, XElement element, FileModel file) {
        XMLRoot = xmlRoot;
        Prefix = prefix;
        IdentificationString = idString;
        File = file;
        Element = element;
    }

    
}