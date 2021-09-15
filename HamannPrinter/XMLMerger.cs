using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HamannPrinter
{
    public static class XMLMerger
    {
        class Merger
        {
            public Action<string[]> LogSink { get; set; } = null;

            private bool _raised = false;

            private XElement _opus = new XElement("opus");
            private XElement _data = new XElement("data");
            private XElement _definitions = new XElement("definitions");
            private XElement _descriptions = new XElement("descriptions");
            private XElement _traditions = new XElement("traditions");
            private XElement _document = new XElement("document");
            private XElement _kommentare = new XElement("kommentare");
            private XElement _marginalien = new XElement("marginalien");
            private XElement _edits = new XElement("edits");


            public Merger()
            {

            }


            // Returns a valid XML-File if there wer no errors and everything's there, otherwise null
            public XDocument GetXDocument()
            {
                _opus.Add(_document);
                _opus.Add(_kommentare);
                _opus.Add(_marginalien);
                _opus.Add(_edits);
                _opus.Add(_traditions);
                _data.Add(_definitions);
                _data.Add(_descriptions);
                _opus.Add(_data);
                if (_botched())
                {
                    Console.WriteLine("Es wurden nicht alle notwendigen Elemente gefunden oder es gab Fehler beim Parsen der XML-Daten.");
                    return null;
                }
                Console.WriteLine("Zusammensetzen des Dokuments erfolgreich!");
                return new XDocument(_opus);
            }

            // Adds a document and parses it, catching all exceptions and writing to standard log
            public bool Add(string path)
            {
                Console.WriteLine(path + " wird geparst...");
                try
                {
                    var currdoc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
                    if (currdoc.Root.Name != "opus" ||
                        !currdoc.Root.Elements().Where(x => x.Name == "data" ||
                                                            x.Name == "document" ||
                                                            x.Name == "kommcat" ||
                                                            x.Name == "marginalien" ||
                                                            x.Name == "traditions" ||
                                                            x.Name == "definitions" ||
                                                            x.Name == "descriptions" ||
                                                            x.Name == "edits").Any())
                    {
                        Console.WriteLine(path + " scheint keine Hamann-Datei zu sein",
                            "Eine Hamann-Datei beginnt stets mit <opus>, gefolgt von <data> oder <document>.");
                        return false;
                    }
                    _dataParser(currdoc.Root, path);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler beim Parsen von " + path + ": ", ex.Message);
                    _raised = true;
                    return false;
                }
            }

            // Adds and parses multiple documents
            public bool Add(params string[] path)
            {
                foreach (var s in path) this.Add(s);
                return true;
            }


            // Validates the document and checks if everything's there.
            private bool _botched()
            {
                if (!_traditions.Elements().Any())
                {
                    Console.WriteLine("Keine Überlieferungsdaten gefunden!");
                    return true;
                }
                if (!_descriptions.Elements().Any())
                {
                    Console.WriteLine("Keine Metadaten der Briefe gefunden!");
                    return true;
                }
                if (!_definitions.Elements().Any())
                {
                    Console.WriteLine("Keine Referenzdaten für Personen- und Ortsnamen gefunden!");
                    return true;
                }
                if (!_edits.Elements().Any())
                {
                    Console.WriteLine("Keine Daten zu den Texteingriffen gefunden!");
                    return true;
                }
                //if (!_data.Elements().Any())
                //{
                //    Console.WriteLine("Fehler beim Zusammensetzen der Datei!");
                //    return true;
                //}
                if (!_kommentare.Elements().Any())
                {
                    Console.WriteLine("Keine Kommentare gefunden!");
                    return true;
                }
                if (!_marginalien.Elements().Any())
                {
                    Console.WriteLine("Keine Marginalien gefunden!");
                    return true;
                }
                if (!_document.Elements().Any())
                {
                    Console.WriteLine("Keine Brieftexte gefunden!");
                    return true;
                }
                if (!_opus.Elements().Any())
                {
                    Console.WriteLine("Fehler beim Zusammensetzen der Datei");
                    return true;
                }
                if (_raised)
                {
                    Console.WriteLine("Es gab ungültige XML-Daten oder Dateinamen.");
                    return true;
                }
                return false;
            }

            // Sorts everything in it's prospective parent elements
            private void _dataParser(XElement data, string path)
            {
                foreach (var x in data.Elements())
                {
                    if (!x.IsEmpty &&
                        String.IsNullOrWhiteSpace(x.Value) &&
                        x.Name == "definitions" ||
                        x.Name == "traditions" ||
                        x.Name == "descriptions" ||
                        x.Name == "edits" ||
                        x.Name == "kommentare" ||
                        x.Name == "marginalien" ||
                        x.Name == "document" ||
                        x.Name == "data")
                    {
                        if (x.Name == "document" && x.Descendants("letterText").Any())
                        {
                            _document.Add(x.Elements());
                            Console.WriteLine("Briefe gefunden.");
                            continue;
                        }
                        if (x.Name == "kommentare" && x.Descendants("kommentar").Any())
                        {
                            _kommentare.Add(x.Elements("kommcat"));
                            Console.WriteLine("Kommentare gefunden.");
                            continue;
                        }
                        if (x.Name == "marginalien" && x.Descendants("marginal").Any())
                        {
                            _marginalien.Add(x.Elements("marginal"));
                            Console.WriteLine("Marginalien gefunden.");
                            continue;
                        }
                        if (x.Name == "traditions" && x.Elements("letterTradition").Any())
                        {
                            _traditions.Add(x.Elements("letterTradition"));
                            Console.WriteLine("Angaben zur Überlieferung gefunden.");
                            continue;
                        }
                        if (x.Name == "edits" && x.Elements("editreason").Any())
                        {
                            _edits.Add(x.Elements("editreason"));
                            Console.WriteLine("Texteingriffe gefunden.");
                            continue;
                        }
                        if (x.Name == "definitions" && x.Elements().Where(s => s.Name == "structureDefs" ||
                                                                                s.Name == "locationDefs" ||
                                                                                s.Name == "handDefs" ||
                                                                                s.Name == "personDefs").Any())
                        {
                            _definitions.Add(x.Elements().Where(s => s.Name == "structureDefs" ||
                                                                               s.Name == "locationDefs" ||
                                                                               s.Name == "handDefs" ||
                                                                               s.Name == "personDefs"));
                            Console.WriteLine("Referenzdaten zu Personen, Quellen und Orten gefunden.");
                            continue;
                        }
                        if (x.Name == "descriptions" && x.Elements("letterDesc").Any())
                        {
                            _descriptions.Add(x.Elements("letterDesc"));
                            Console.WriteLine("Metadaten für Briefe gefunden.");
                            continue;
                        }
                        if (x.Name == "data")
                        {
                            _dataParser(x, path);
                            Console.WriteLine("<data> gefunden. Rekursion...");
                        }
                    }
                }
            }
        }

        public class Concatinator
        {
            private string SourceDirectory { get; set; }
            public string HamannXmlFile { get; set; }

            public Concatinator(string sourcedir)
            {
                SourceDirectory = sourcedir;
                getFilePath(SourceDirectory);
            }

            private void getFilePath(string source)
            {
                if (Directory.Exists(source))
                {
                    var x = Directory.GetFiles(source + @"\", "*.xml");
                    if (x.Length == 0)
                    {
                        Console.WriteLine("Keine XML-Dateien in " + source + @"\" + " gefunden.");
                    }
                    {
                        foreach (var f in x)
                        {
                            Console.WriteLine("Datei " + f + " gefunden.");
                        }
                        var ret = ParseFiles(x);
                        if (ret == null)
                        {
                            Console.WriteLine("Parsen der Dateien fehlgeschlagen. Sind die Briefe, Metadaten und Referenzdaten vorhanden?");
                        }
                        else
                        {
                            Console.WriteLine("Parsen der Dateien erfolgreich. Briefe, Metadaten und Referenzen gefunden.");
                            HamannXmlFile = ParseConcatinated(ret);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Verzeichnis nicht existent");
                }
            }

            private string ParseConcatinated(XDocument document)
            {
                try
                {
                    document.Save(SourceDirectory + @"\HAMANN.xml");
                    Console.WriteLine("Die Quelldatei wurde unter " + SourceDirectory + @"\HAMANN.xml gespeichert.");
                    return SourceDirectory + @"\HAMANN.xml";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Das Speichern von " + SourceDirectory + @"\HAMANN.xml" + " ist fehlgeschlagen", ex.Message);
                    return "";
                }
            }

            static public XDocument ParseFiles(string[] files)
            {

                var merge = new Merger();
                merge.Add(files);
                return merge.GetXDocument();
            }
        }
    }
}
