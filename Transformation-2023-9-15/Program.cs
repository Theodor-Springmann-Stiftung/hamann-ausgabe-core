using System.IO;
using System.Xml.Linq;
// See https://aka.ms/new-console-template for more information
const string XML_PATH = "D:/Simon/source/hamann-xml/transformations_2023-9-14_test/";
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

List<(string, XDocument)> Documents = new();

foreach (var f in xmls) {
    Documents.Add((f, XDocument.Load(f, LoadOptions.PreserveWhitespace)));
    Console.WriteLine("" + f);
}


void SaveHamannFile(XElement element, string basefilepath, string oldfile) {
        var filename = "hamann_" + ".xml";
        var path = Path.Combine(basefilepath, filename);

        if (!Directory.Exists(basefilepath))
            Directory.CreateDirectory(basefilepath);
        using (var targetStream = System.IO.File.Create(path))
            element.Save(targetStream, SaveOptions.DisableFormatting);
    
    }