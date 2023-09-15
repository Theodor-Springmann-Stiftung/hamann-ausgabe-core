using System.Xml;
using System.Xml.Linq;

public static class AutopsicNumberTransform {
    // Checks are done, we begin here
    // State
    public static List<(string, XDocument, bool)> Documents = new();
    static Dictionary<string, string> OldNewIndex = new();
    static Dictionary<string, List<XElement>> Intlinks = new();
    public static Dictionary<string, List<XElement>> Marginals = new();
    static Dictionary<string, List<XElement>> LetterTexts = new();
    static Dictionary<string, List<XElement>> LetterTraditions = new();
    static Dictionary<string, List<XElement>> LetterDescs = new();

    public static void Collect(string[] xmls) {
        
        List<XElement> Autopsic = new();

        foreach (var f in xmls) {
            XmlReaderSettings set = new XmlReaderSettings();
            set.IgnoreWhitespace = false;
            set.CheckCharacters = false;
            using (FileStream fs = File.Open(f, FileMode.Open)) {    
            var d = XDocument.Load(fs, LoadOptions.PreserveWhitespace);
            var affected = false;

            var intlinks = d.Descendants("intlink");
            if (intlinks != null && intlinks.Any()) {
                foreach (var e in intlinks) {
                    if (e.HasAttributes && e.Attribute("letter") != null) {
                        int letter = -1;
                        if (Int32.TryParse(e.Attribute("letter").Value, out letter) && letter > 368) {
                            if (!Intlinks.ContainsKey(e.Attribute("letter").Value)) Intlinks.Add(e.Attribute("letter").Value, new());
                            Intlinks[e.Attribute("letter").Value].Add(e);
                            Console.WriteLine("intlink: " + e.ToString() + ", document: " + f);
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

    public static void Transform() {
    List<Dictionary<string, List<XElement>>> Collections = new() { Intlinks, Marginals, LetterTexts, LetterTraditions, LetterDescs };
        foreach (var number in OldNewIndex) {
            Console.Write(number.Key + " -> " + number.Value);
            foreach (var c in Collections) {
                if (c != null && c.ContainsKey(number.Key)) {
                    foreach (var v in c[number.Key]) {
                        if (v.HasAttributes && v.Attribute("letter") != null) {
                            v.Attribute("letter").Value = number.Value;
                        } else if (v.HasAttributes && v.Attribute("ref") != null) {
                            v.Add(new XAttribute("letter", number.Value));
                            v.Attribute("ref").Remove();

                            if (!v.IsEmpty && v.Element("autopsic") != null) {
                                v.Element("autopsic").Remove();
                            }
                        } else if (v.HasAttributes && v.Attribute("index") != null) {
                            v.Add(new XAttribute("letter", number.Value));
                            v.Attribute("index").Remove();
                        }
                        if (v.HasAttributes && v.Attribute("autopsic") != null) {
                            v.Attribute("autopsic").Remove();
                        }
                    }
                }
            }
        }
    }
}