using System.Xml;
using System.Xml.Linq;

public static class FileOperations {
    public static void SaveFile(List<(string, XDocument, bool)> Documents, string dest) {
        foreach (var d in Documents) {
            if (d.Item3) {
                if (!Directory.Exists(dest)) {
                    Directory.CreateDirectory(dest);
                }
                var filenameold = d.Item1.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).LastOrDefault();
                if (filenameold == null) return;
                var path = Path.Combine(dest, filenameold);
                // element.Save(path, SaveOptions.DisableFormatting);
                XmlWriterSettings set = new XmlWriterSettings() {
                    CheckCharacters = false,
                    Encoding = System.Text.Encoding.UTF8,
                };
                using (XmlWriter writer = XmlWriter.Create(path, set)) {
                    d.Item2.Save(writer);
                }
                // using (var targetStream = System.IO.File.Create(path)) {
                //     element.Save(targetStream, SaveOptions.DisableFormatting); 
                //}
            }
        }
    }

    // Gets XML data if branch name is correct (to prevent data errors)
    public static string[] GetXMLs(string path, string git_path, string? branch_name) {
        // BASIC CHECKS
        if (branch_name != null) {
            if (File.Exists(git_path + ".git/HEAD")) {
                var text = File.ReadAllText(git_path + ".git/HEAD").Trim();
                if (!text.EndsWith(branch_name)) 
                    throw new("Not it the Branch " + branch_name);
            } else throw new("Specified Paths do not exist");
        }
        
        if (!Directory.Exists(path)) throw new("Directory does not exist!");

        var xmls = Directory.GetFiles(path, "*.xml");
        if (xmls == null || !xmls.Any())  throw new("No XML Data fonund!");
        
        return xmls;
    }
}