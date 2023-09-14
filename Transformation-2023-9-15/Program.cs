using System.IO;
using System.Xml.Linq;
// See https://aka.ms/new-console-template for more information
const string XML_PATH = "D:/Simon/source/hamann-xml/base/";
const string DEST_PATH = "D:/Simon/source/hamann-xml/transformations_2023-9-14_test/";
const string GIT_PATH = "../../hamann-xml/";
const string BRANCH_NAME = "testdata";

if (File.Exists(GIT_PATH + ".git/HEAD") || !Directory.Exists(XML_PATH)) {
    var text = File.ReadAllText(GIT_PATH + ".git/HEAD").Trim();
    if (!text.EndsWith(BRANCH_NAME)) {
        throw new("Not it the Branch " + BRANCH_NAME);
    }
} else {
    throw new("Specified Paths do not exist");
}

var xmls = Directory.GetFiles(XML_PATH, "*.xml");

if (xmls == null || !xmls.Any()) {
    throw new("No XML Data fonund!");
}

// Checks are done, we begin here
// State
List<(string, XDocument)> Documents = new();
Dictionary<string, string> OldNewIndex = new();
Dictionary<string, List<XElement>> Intlinks = new();
Dictionary<string, List<XElement>> Marginals = new();
Dictionary<string, List<XElement>> LetterTexts = new();
Dictionary<string, List<XElement>> LetterTraditions = new();
Dictionary<string, List<XElement>> LetterDescs = new();

foreach (var f in xmls) {
    var d = XDocument.Load(f, LoadOptions.PreserveWhitespace);
    Documents.Add((f, d));

    var intlinks = d.Descendants("intlink");
    if (intlinks != null && intlinks.Any()) {
        foreach (var e in intlinks) {
            if (e.HasAttributes && e.Attribute("letter") != null) {
                if (!Intlinks.ContainsKey(e.Attribute("letter").Value)) Intlinks.Add(e.Attribute("letter").Value, new());
                Intlinks[e.Attribute("letter").Value].Add(e);
            }
        }
    }

    var marginals = d.Descendants("marginal");
    if (marginals != null && marginals.Any()) {
        foreach (var e in marginals) {
            if (e.HasAttributes && e.Attribute("letter") != null) {
                if (!Marginals.ContainsKey(e.Attribute("letter").Value)) Marginals.Add(e.Attribute("letter").Value, new());
                    Marginals[e.Attribute("letter").Value].Add(e);
            }
        }
    }

    var lettertexts = d.Descendants("letterText");
    if (lettertexts != null && lettertexts.Any()) {
        foreach (var e in lettertexts) {
            if (e.HasAttributes && e.Attribute("index") != null) {
                if (!LetterTexts.ContainsKey(e.Attribute("index").Value)) LetterTexts.Add(e.Attribute("index").Value, new());
                    LetterTexts[e.Attribute("index").Value].Add(e);
            }
        }
    }

    var lettertraditions = d.Descendants("letterTradition");
    if (lettertraditions != null && lettertraditions.Any()) {
        foreach (var e in lettertraditions) {
            if (e.HasAttributes && e.Attribute("ref") != null) {
                if (!LetterTraditions.ContainsKey(e.Attribute("ref").Value)) LetterTraditions.Add(e.Attribute("ref").Value, new());
                LetterTraditions[e.Attribute("ref").Value].Add(e);
            }
        }
    }

    var letterdescs = d.Descendants("letterDesc");
    if (letterdescs != null && letterdescs.Any()) {
        foreach (var e in letterdescs) {
            if (e.HasAttributes && e.Attribute("ref") != null) {
                if (!LetterDescs.ContainsKey(e.Attribute("ref").Value)) LetterDescs.Add(e.Attribute("ref").Value, new());
                    LetterDescs[e.Attribute("ref").Value].Add(e);

                if (e.Element("autopsic") != null && e.Element("autopsic").HasAttributes && e.Element("autopsic").Attribute("value") != null) 
                    OldNewIndex.Add(e.Attribute("ref").Value, e.Element("autopsic").Attribute("value").Value);
                
            }
        }
    }
}



void SaveFile(XDocument element, string basefilepath, string oldfile) {
    var filenameold = oldfile.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).LastOrDefault();
    if (filenameold == null) return;
    var filename = oldfile + "_transform_" + ".xml";
    var path = Path.Combine(basefilepath, filename);

    if (!Directory.Exists(basefilepath))
        Directory.CreateDirectory(basefilepath);
    using (var targetStream = System.IO.File.Create(path)) {
        element.Save(targetStream, SaveOptions.DisableFormatting); 
    }
}