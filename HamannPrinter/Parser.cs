using System;
using HaDocument.Interfaces;
using HaDocument;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace HamannPrinter
{
    public class Parser
    {
        class Options : IHaDocumentOptions
        {
            public string HamannXMLFilePath { get; set; }
            public string[] AvailableVolumes { get; set; } = { "1", "2", "3", "4", "5", "6", "7" };
            public bool NormalizeWhitespace { get; set; } = true;

            public (int, int) AvailableYearRange {get; set; } = (1751, 1788);

            public Options(string HamannXmlPath)
            {
                HamannXMLFilePath = HamannXmlPath;
            }
        }

        public class DocOptions
        {

            public string OutputDir { get; set; }
            public string VolumesOutputDir { get; set; }
            public string Editionsrichtlinien { get; set; }
            public string LineHight { get; set; } = "280";
            public string SmallDistance { get; set; } = "140";
            public string MiddleLineHight { get; set; } = "224";
            public string FontSize { get; set; } = "20";
            public string MiddleFontSize { get; set; } = "16";
            public string SmallFontSize { get; set; } = "12";
            public string BigFontSize { get; set; } = "32";
            public int MarginTop { get; set; } = 1416;
            public UInt32 MarginRight { get; set; } = 2600U;
            public int MarginBottom { get; set; } = 2132;
            public UInt32 MarginLeft { get; set; } = 1984U;
            public UInt32 MarginFooter { get; set; } = 1417U;
            public string ColumnDistance { get; set; } = "560";
            public string FooterToText { get; set; } = "420";
            public string NormalFont { get; set; } = "Linux Libertine G";
            public string SpecialFont { get; set; } = "Linux Biolinum";
            public string SuperValue { get; set; } = "6";
            public string SubValue { get; set; } = "-4";
            public (int, int) Years { get; set; }

            public DocOptions((int, int) years, string outputDir, string editionsRichtlinien)
            {
                Editionsrichtlinien = editionsRichtlinien;
                Years = years;
                OutputDir = outputDir;
                VolumesOutputDir = System.IO.Directory.CreateDirectory(outputDir + @"bände_register\").FullName;
            }
        }

        public static class Logger
        {
            public static StringBuilder LogString = new StringBuilder();
            public static void Out(string str)
            {
                Console.WriteLine(str);
                LogString.Append(str).Append(Environment.NewLine);
            }
        }

        public void MakeDocuments(Confix confix)
        {
            var hamannDoc = Document.Create(new Options(confix.HamannXmlPath));
            DocOptions docOpt = new DocOptions(confix.Years, confix.OutputPath, confix.Editionsrichtlinien);
            CheckXML(confix, docOpt, hamannDoc);
            Coordinator(docOpt, hamannDoc, hamannDoc, docOpt.Years, confix.LettersDocx, confix.VolumeDocx, confix.RegisterDocx);
            Helper.Ok("Fertig!");
            Environment.Exit(0);
        }

        public void CheckXML(Confix confix, DocOptions docOpt, ILibrary hamannDoc)
        {
            var errors = new StringBuilder();
            var document = XDocument.Load(confix.HamannXmlPath, LoadOptions.SetLineInfo);

            var kommentare = document.Descendants("kommentar");
            var subsections = document.Descendants("subsection");
            var intlinks = document.Descendants("intlink");
            var links = document.Descendants("link");
            var marginals = document.Descendants("marginal");
            var hands = document.Descendants("hand");
            var edits = document.Descendants("edit");
            var editreasons = document.Descendants("editreason");

            var ids = new HashSet<String>();
            foreach (var kommentar in kommentare)
            {
                if (kommentar.HasAttributes && kommentar.Attribute("id") != null)
                {
                    if (!ids.Contains(kommentar.Attribute("id").Value))
                        ids.Add(kommentar.Attribute("id").Value);
                    else
                    {
                        errors.AppendLine("Kommentar-ID " + kommentar.Attribute("id").Value + " doppelt vergeben!");
                        errors.AppendLine();
                    }
                }   
            }

            foreach (var kommentar in subsections)
            {
                if (kommentar.HasAttributes && kommentar.Attribute("id") != null)
                {
                    if (!ids.Contains(kommentar.Attribute("id").Value))
                        ids.Add(kommentar.Attribute("id").Value);
                    else
                    {
                        errors.AppendLine("Kommentar-ID " + kommentar.Attribute("id").Value + " doppelt vergeben!");
                        errors.AppendLine();
                    }
                }
            }

            foreach (var link in intlinks)
            {
                if (link.HasAttributes && link.Attribute("letter") != null)
                {
                    if (hamannDoc.Letters.ContainsKey(link.Attribute("letter").Value))
                    {
                        if (link.Attribute("page") != null)
                        {
                            var lettertext = XElement.Parse(hamannDoc.Letters[link.Attribute("letter").Value].Element);
                            var lettertext2 = XElement.Parse(hamannDoc.Traditions[link.Attribute("letter").Value].Element);
                            var page = lettertext.Descendants("page").Where(x => x.Attribute("index").Value.ToString() == link.Attribute("page").Value.ToString()).Union(lettertext2.Descendants("page").Where(x => x.Attribute("index").Value.ToString() == link.Attribute("page").Value.ToString()));
                            if (page.Any()) 
                            {
                                if (link.Attribute("line") != null)
                                {
                                    var elementsafter = lettertext.Descendants().SkipWhile(x => x.Name != "page" || !x.Attributes("index").Any() || x.Attribute("index").Value != link.Attribute("page").Value).TakeWhile(x => x.Name.ToString() != "page" || (x.Name.ToString() == "page" && x.Attribute("index").Value == link.Attribute("page").Value));
                                    var elementsafter2 = lettertext2.Descendants().SkipWhile(x => x.Name != "page" || !x.Attributes("index").Any() || x.Attribute("index").Value != link.Attribute("page").Value).TakeWhile(x => x.Name.ToString() != "page" || (x.Name.ToString() == "page" && x.Attribute("index").Value == link.Attribute("page").Value));
                                    var line = elementsafter.Where(x => x.Name == "line" && x.HasAttributes && x.Attributes("index").Any() && x.Attribute("index").Value == link.Attribute("line").Value).Union(elementsafter2.Where(x => x.Name == "line" && x.HasAttributes && x.Attributes("index").Any() && x.Attribute("index").Value == link.Attribute("line").Value));
                                    if (!line.Any())
                                    {
                                        var info = (IXmlLineInfo)link;
                                        errors.AppendLine(link.ToString() + " Z. " + info.LineNumber + " verweist auf eine nicht existierende Zeile (" + link.Attribute("line").Value + ") !");
                                        errors.AppendLine();
                                    }
                                }
                            }
                            else
                            {
                                var info = (IXmlLineInfo)link;
                                errors.AppendLine(link.ToString() + " Z. " + info.LineNumber + " verweist auf eine nicht existierende Seite (" + link.Attribute("page").Value + ") !");
                                errors.AppendLine();
                            }
                        }
                    }
                    else 
                    {
                        var info = (IXmlLineInfo)link;
                        errors.AppendLine(link.ToString() + " Z. " + info.LineNumber + " verweist auf einen nicht existierenden Brief (" + link.Attribute("letter").Value + ") !");
                        errors.AppendLine();
                    }
                }
                else
                {
                    var info = (IXmlLineInfo)link;
                    errors.AppendLine(link.ToString() + " Z. " + info.LineNumber + " hat kein letter-Attribut!");
                    errors.AppendLine();
                }
            }

            var marginalset = new HashSet<String>();
            foreach (var link in marginals)
            {
                if (link.HasAttributes && link.Attribute("index") != null) {
                    if (marginalset.Contains(link.Attribute("index").Value.ToString())) {
                        var info = (IXmlLineInfo)link;
                        errors.AppendLine(link.ToString() + " Z. " + info.LineNumber + " hate eine bereits vergebene Indexnummer und wird ignoriert. (" + link.Attribute("index").Value + ") !");
                        errors.AppendLine();
                    } else 
                        marginalset.Add(link.Attribute("index").Value.ToString());
                }
                if (link.HasAttributes && link.Attribute("letter") != null)
                {
                    if (hamannDoc.Letters.ContainsKey(link.Attribute("letter").Value))
                    {
                        if (link.Attribute("page") != null)
                        {
                            var lettertext = XElement.Parse(hamannDoc.Letters[link.Attribute("letter").Value].Element);
                            var page = lettertext.Descendants("page").Where(x => x.Attribute("index").Value.ToString() == link.Attribute("page").Value.ToString());
                            if (page.Any())
                            {
                                if (link.Attribute("line") != null)
                                {
                                    var elementsafter = lettertext.Descendants().SkipWhile(x => x.Name != "page" || !x.Attributes("index").Any() || x.Attribute("index").Value != link.Attribute("page").Value).TakeWhile(x => x.Name.ToString() != "page" || (x.Name.ToString() == "page" && x.Attribute("index").Value == link.Attribute("page").Value));
                                    var line = elementsafter.Where(x => x.Name == "line" && x.HasAttributes && x.Attributes("index").Any() && x.Attribute("index").Value.ToString() == link.Attribute("line").Value.ToString());
                                    if (!line.Any())
                                    {
                                        var info = (IXmlLineInfo)link;
                                        errors.AppendLine(link.ToString() + " Z. " + info.LineNumber + " verweist auf eine nicht existierende Zeile (" + link.Attribute("line").Value + ") !");
                                        errors.AppendLine();
                                    }
                                }
                            }
                            else
                            {
                                var info = (IXmlLineInfo)link;
                                errors.AppendLine(link.ToString() + " Z. " + info.LineNumber + " verweist auf eine nicht existierende Seite (" + link.Attribute("page").Value + ") !");
                                errors.AppendLine();
                            }
                        }
                    }
                    else
                    {
                        var info = (IXmlLineInfo)link;
                        errors.AppendLine(link.ToString() + " Z. " + info.LineNumber + " verweist auf einen nicht existierenden Brief (" + link.Attribute("letter").Value + ") !");
                        errors.AppendLine();
                    }
                }
                else
                {
                    var info = (IXmlLineInfo)link;
                    errors.AppendLine(link.ToString() + " Z. " + info.LineNumber + " hat kein letter-Attribut!");
                    errors.AppendLine();
                }
            }

            foreach (var link in links)
            {
                if (link.HasAttributes && link.Attribute("ref") != null)
                {
                    if (!ids.Contains(link.Attribute("ref").Value))
                    {
                        var info = (IXmlLineInfo)link;
                        errors.AppendLine(link.ToString() + " Z. " + info.LineNumber + " verweist auf einen nicht existierenden Kommentar (" + link.Attribute("ref").Value + ") !");
                        errors.AppendLine();
                    }
                }

                if (link.HasAttributes && link.Attribute("subref") != null)
                {
                    if (!ids.Contains(link.Attribute("subref").Value))
                    {
                        var info = (IXmlLineInfo)link;
                        errors.AppendLine(link.ToString() + " Z. " + info.LineNumber + " verweist auf einen nicht existierenden Kommentar (" + link.Attribute("subref").Value + ") !");
                        errors.AppendLine();
                    }
                }
            }


            foreach (var hand in hands)
            {
                if (hand.HasAttributes && hand.Attribute("ref") != null)
                {
                    if (!hamannDoc.HandPersons.ContainsKey(hand.Attribute("ref").Value))
                    {
                        var info = (IXmlLineInfo)hand;
                        errors.AppendLine(hand.ToString() + " Z. " + info.LineNumber + " verweist auf einen nicht angelegte Person in den Handrefs (" + hand.Attribute("ref").Value + ") !");
                        errors.AppendLine();
                    }
                }
            }

            foreach (var edit in edits)
            {
                if (edit.HasAttributes && edit.Attribute("ref") != null)
                {
                    if (!hamannDoc.Editreasons.ContainsKey(edit.Attribute("ref").Value))
                    {
                        var info = (IXmlLineInfo)edit;
                        errors.AppendLine(edit.ToString() + " Z. " + info.LineNumber + " verweist auf eine nicht existierende Editreason (" + edit.Attribute("ref").Value + ") !");
                        errors.AppendLine();
                    }
                }
            }

            //foreach (var link in links)
            //{
            //    if (link.HasAttributes && link.Attribute("subref") != null) 
            //    {
            //        if (ids.Contains(link.Attribute("subref").Value))
            //            ids.Remove(link.Attribute("subref").Value);
            //    }
            //    else if (link.HasAttributes && link.Attribute("ref") != null)
            //    {
            //        if (ids.Contains(link.Attribute("ref").Value))
            //            ids.Remove(link.Attribute("ref").Value);
            //    }

            //    if (ids.Any())
            //    {
            //        foreach (var id in ids)
            //        {
            //            errors.AppendLine("WARNUNG: Auf den Kommentar mit der ID " + id + " wird nirgends verwiesen.");
            //            errors.AppendLine();
            //        }
            //    }
            //}

            System.IO.File.WriteAllText(docOpt.OutputDir + "errors.txt", errors.ToString());
        }

        public void Coordinator(DocOptions docOpt, ILibrary hamannDoc, ILibrary lib, (int, int) years, bool? letterDocs, bool? volDocs, bool? registerDocs)
        {
            /* koordiniert das Erstellen der einzelnen Dokumenttypen/-sorten*/
            //try
            //{
                var h2w = new Hamann2Word(hamannDoc, docOpt);
                if (letterDocs == true || letterDocs == false) // DEV
                {
                    Logger.Out("Erstelle docx für einzelbriefe");
                    h2w.MakeLetterDocuments(lib, years);
                }
                if (volDocs == true)
                {
                    Logger.Out("Erstelle docx für BandDateien");
                    h2w.MakeYearDocuments(lib, years);
                }
                if (registerDocs == true)
                {
                    Logger.Out("Erstelle docx für Register");
                    Hamann2Word.MakeRegisterComms();
                }
            //}
            //catch (Exception ex)
            //{
            //    Logger.Out(ex.Message);
            //    System.IO.File.WriteAllText(Hamann2Word.OutputDir + "logfile.txt", Logger.LogString.ToString());
            //    Environment.Exit(1);               
            //}
            System.IO.File.WriteAllText(Hamann2Word.OutputDir + "logfile.txt", Logger.LogString.ToString());
        }

    }
}
