using System.IO;
using System.Security;
using System.Xml;
using System.Xml.Linq;
// See https://aka.ms/new-console-template for more information
const string XML_PATH = "C:/Users/simon/source/hamann-xml/base/";
const string DEST_PATH = "C:/Users/simon/source/hamann-xml/transformations_2023-9-14_test/";
const string GIT_PATH = "C:/Users/simon/source/hamann-xml/";
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
List<(string, XDocument, bool)> Documents = new();
Dictionary<string, string> OldNewIndex = new();
Dictionary<string, List<XElement>> Intlinks = new();
Dictionary<string, List<XElement>> Marginals = new();
Dictionary<string, List<XElement>> LetterTexts = new();
Dictionary<string, List<XElement>> LetterTraditions = new();
Dictionary<string, List<XElement>> LetterDescs = new();

List<XElement> Autopsic = new();

foreach (var f in xmls) {
    XmlReaderSettings set = new XmlReaderSettings();
    set.IgnoreWhitespace = false;
    set.CheckCharacters = false;
    using (FileStream fs = File.Open(f, FileMode.Open)) {
    using (var r = new XmlTextReader(fs) { Normalization = false, WhitespaceHandling = WhitespaceHandling.All, EntityHandling = EntityHandling.ExpandCharEntities}) {
    
    var d = XDocument.Load(r);
    var affected = false;

    var intlinks = d.Descendants("intlink");
    if (intlinks != null && intlinks.Any()) {
        foreach (var e in intlinks) {
            if (e.HasAttributes && e.Attribute("letter") != null) {
                int letter = -1;
                if (Int32.TryParse(e.Attribute("letter").Value, out letter) && letter > 368) {
                    if (!Intlinks.ContainsKey(e.Attribute("letter").Value)) Intlinks.Add(e.Attribute("letter").Value, new());
                    Intlinks[e.Attribute("letter").Value].Add(e);
                    Console.WriteLine(e.ToString());
                    affected = true;
                }
            }
        }
    }

    var marginals = d.Descendants("marginal");
    if (marginals != null && marginals.Any()) {
        foreach (var e in marginals) {
            if (e.HasAttributes && e.Attribute("letter") != null) {
                if (!Marginals.ContainsKey(e.Attribute("letter").Value)) Marginals.Add(e.Attribute("letter").Value, new());
                    Marginals[e.Attribute("letter").Value].Add(e);
                    affected = true;
            }
        }
    }

    var lettertexts = d.Descendants("letterText");
    if (lettertexts != null && lettertexts.Any()) {
        foreach (var e in lettertexts) {
            if (e.HasAttributes && e.Attribute("index") != null) {
                if (!LetterTexts.ContainsKey(e.Attribute("index").Value)) LetterTexts.Add(e.Attribute("index").Value, new());
                    LetterTexts[e.Attribute("index").Value].Add(e);
                    affected = true;
            }
        }
    }

    var lettertraditions = d.Descendants("letterTradition");
    if (lettertraditions != null && lettertraditions.Any()) {
        foreach (var e in lettertraditions) {
            if (e.HasAttributes && e.Attribute("ref") != null) {
                if (!LetterTraditions.ContainsKey(e.Attribute("ref").Value)) LetterTraditions.Add(e.Attribute("ref").Value, new());
                LetterTraditions[e.Attribute("ref").Value].Add(e);
                affected = true;
            }
        }
    }

    var letterdescs = d.Descendants("letterDesc");
    if (letterdescs != null && letterdescs.Any()) {
        foreach (var e in letterdescs) {
            if (e.HasAttributes && e.Attribute("ref") != null) {
                if (!LetterDescs.ContainsKey(e.Attribute("ref").Value)) LetterDescs.Add(e.Attribute("ref").Value, new());
                    LetterDescs[e.Attribute("ref").Value].Add(e);

                if (e.Element("autopsic") != null && e.Element("autopsic").HasAttributes && e.Element("autopsic").Attribute("value") != null) {
                    OldNewIndex.Add(e.Attribute("ref").Value, e.Element("autopsic").Attribute("value").Value);
                    Autopsic.Add(e.Element("autopsic"));
                    affected = true;
                }
                
            }
        }
    }

    Documents.Add((f, d, affected));
    }
    }
}

List<Dictionary<string, List<XElement>>> Collections = new() { Intlinks, Marginals, LetterTexts, LetterTraditions, LetterDescs };
foreach (var number in OldNewIndex) {
    if (number.Key == number.Value) continue;
    foreach (var c in Collections) {
        if (c != null && c.ContainsKey(number.Key)) {
            foreach (var v in c[number.Key]) {
                if (v.HasAttributes && v.Attribute("letter") != null) {
                    v.Attribute("letter").Value = number.Value;
                } else if (v.HasAttributes && v.Attribute("ref") != null) {
                    v.Attribute("ref").Value = number.Value;
                } else if (v.HasAttributes && v.Attribute("index") != null) {
                    v.Attribute("index").Value = number.Value;
                }
                // NOT POSSIBLE:
            //     if (v.HasAttributes && v.Attribute("autopsic") != null) {
            //         v.Attribute("autopsic").Remove();
            //     }
            }
        }
    }
}

foreach (var d in Documents) {
    //if (d.Item3) SaveFile(d.Item2, DEST_PATH, d.Item1);
}

void SaveFile(XDocument element, string basefilepath, string oldfile) {
    if (!Directory.Exists(basefilepath)) {
        Directory.CreateDirectory(basefilepath);
    }
    var filenameold = oldfile.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).LastOrDefault();
    if (filenameold == null) return;
    var filename = oldfile;
    var path = Path.Combine(basefilepath, filename);

    if (!Directory.Exists(basefilepath))
        Directory.CreateDirectory(basefilepath);
    File.WriteAllText(path, element.ToString());
    // XmlWriterSettings set = new XmlWriterSettings() {
    //     CheckCharacters = false
    // };
    // using (XmlTextWriter wr = new XmlTextWriter(path, System.Text.Encoding.UTF8) { Formatting = System.Xml.Formatting.None }) {
    //     element.Save(wr);
    // }
    // using (var targetStream = System.IO.File.Create(path)) {
    //     element.Save(targetStream, SaveOptions.DisableFormatting); 
    //}
}