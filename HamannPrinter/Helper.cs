using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using static HamannPrinter.Hamann2Word;
using static HamannPrinter.Parser;
using Comment = HaDocument.Models.Comment;

namespace HamannPrinter
{
    static class Helper
    {

        public static void Warn(string message)
        {
                MessageBox.Show(message,
                "Confirmation",
                MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
        }

        public static void Ok(string message)
        {
            MessageBox.Show(message,
            "Confirmation",
            MessageBoxButton.OK,
            MessageBoxImage.Asterisk);
        }

        public static MessageBoxResult SayYesNo(string message)
        {
            var result = MessageBox.Show(message,
            "Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
            return result;
        }

        public static String MakeZHString(LetterObj letter, bool withZH)
        {
            // TODO ONLY IF ZH != mull
            if (letter.Meta.ZH != null)
            {
                string bd = GetRoman(letter.Meta.ZH.Volume);
                string lastPg = GetLastPg(letter);
                string firstPg = letter.Meta.ZH.Page;
                string zh = withZH ? "ZH\u202F" : "";
                if (lastPg == null || lastPg == firstPg)
                {
                    zh += bd + " " + firstPg;
                }
                else
                {
                    zh += bd + " " + firstPg + "\u2012" + lastPg;
                }
                return zh;
            }
            else if (withZH)
            {
                return "nicht in ZH";
            }
            else
            {
                return "";
            }
            
        }

        public static WordprocessingDocument CreateOutputFile(string path)
        {
            try
            {
                var WordDoc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document);
                return WordDoc;
            }
            catch (IOException)
            {
                Logger.Out("Die Datei " + path + " kann nicht erstellt werden. Vielleicht ist sie geöffnet?\nBitte schließen und enter Drücken oder das Programm schließen.");
                Warn("Die Datei " + path + " kann nicht erstellt werden. Vielleicht ist sie geöffnet?\nBitte schließen und enter Drücken oder das Programm schließen.");
                return CreateOutputFile(path);
            }
        }
        public static void MakeFinalPageBreak(LetterObj letter)
        {
            var srcPara = new Paragraph(new Run(new Break() { Type = BreakValues.Page }));
            GetLastPara(letter.WordDoc).InsertAfterSelf(srcPara);
        }

        public static void CorrectMarginLastSection(WordprocessingDocument Doc, bool nostandartlayout = false)
        {
            /*Das muss nach den footern angefügt werden, ich habe keine Ahnung, warum; eigentlich MUSS das auch vorher eingefügt werden können. Ich bin auf jeden Fall unfähig
            das richtig zu tun und füge diese Foramtierung für die letzte section hier ein, weil sonst komische leerseiten am ende der Dokumente auftauchen.*/
            //sectionproperties erzeugen 
            SectionProperties sectionProperties = Doc.MainDocumentPart.Document.Body.AppendChild<SectionProperties>(new SectionProperties());
            //Seitenränder definieren
            if (nostandartlayout)
            {
                PageMargin pageMargin = new PageMargin() { Top = MarginTop, Right = MarginLeft, Bottom = MarginBottom, Left = MarginLeft, Footer = MarginFooter };
                sectionProperties.PrependChild(pageMargin);
            }
            else
            {
                PageMargin pageMargin = new PageMargin() { Top = MarginTop, Right = MarginRight, Bottom = MarginBottom, Left = MarginLeft, Footer = MarginFooter };
                sectionProperties.PrependChild(pageMargin);
            }
            //Columnen bestimmen
            sectionProperties.PrependChild<Columns>(new Columns() { ColumnCount = 1 });
            sectionProperties.AppendChild<SectionType>(new SectionType() { Val = SectionMarkValues.Continuous });
            var cols = sectionProperties.GetFirstChild<Columns>();
            cols.ColumnCount = 1;
        }

        public static Paragraph GetLastPara(WordprocessingDocument wordDoc)
        {
            Paragraph lastParagraph = wordDoc.MainDocumentPart.Document.Body.Elements<Paragraph>().Last();
            return lastParagraph;
        }

        public static string GetLastPg(LetterObj letter)
        {
            string page = null;
            if (letter.Text.Descendants("page").Any() && letter.Text.Descendants("page").Last().Attributes("autopsic").Any())
                page = letter.Text.Descendants("page").Last().Attribute("autopsic").Value;
            else if (letter.Text.Descendants("page").Any() && letter.Text.Descendants("page").Last().Attributes("index").Any())
                page = letter.Text?.Descendants("page")?.LastOrDefault()?.Attribute("index").Value;
            else
                page = null;

            return page;
        }

        public static string GetRoman(string index)
        {
            switch (index)
            {
                case "1":
                    return "I";
                case "2":
                    return "II";
                case "3":
                    return "III";
                case "4":
                    return "IV";
                case "5":
                    return "V";
                case "6":
                    return "VI";
                case "7":
                    return "VII";
                default:
                    var number = 0;
                    if (!Int32.TryParse(index, out number)) {
                        Logger.Out("Kann BandZahl " + index + "nicht in römische Zahl auflösen.");
                        return "?";
                    }
                    else {
                        return ToRoman(number);
                    }
            }
        }

        private static Dictionary<char, int> RomanMap = new Dictionary<char, int>()
        {
                {'I', 1},
                {'V', 5},
                {'X', 10},
                {'L', 50},
                {'C', 100},
                {'D', 500},
                {'M', 1000}
        };

        public static int RomanToInteger(string roman)
        {
            var ro = roman.ToUpper();
            int number = 0;
            for (int i = 0; i < roman.Length; i++)
            {
                if (RomanMap.ContainsKey(ro[i]) && (i + 1 >= ro.Length || RomanMap.ContainsKey(ro[i + 1])))
                {
                    if (i + 1 < ro.Length && RomanMap[ro[i]] < RomanMap[ro[i + 1]])
                    {
                        number -= RomanMap[ro[i]];
                    }
                    else
                    {
                        number += RomanMap[ro[i]];
                    }
                }
                else return 0;
            }
            return number;
        }

        public static int RomanOrNumberToInt(string number)
        {
            var a = 0;
            if (Int32.TryParse(number, out a)) return a;
            else return RomanToInteger(number);
        }
        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) return string.Empty;
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            return string.Empty;
        }

        public static string GetHighestParentNode(XNode xnode)
        {
            while (xnode.Parent != null)
            {
                xnode = xnode.Parent;
            }
            XElement xelem = xnode as XElement;
            return xelem.Name.LocalName;
        }

        public static string GetPerson(string index)
        {
            var person = Letters.Persons[index];//.Where(x => x.Value.Index == index).First().Value;
            if (Letters.Persons.Where(x => x.Value.Index == index).First().Key != person.Index)
            {
                Logger.Out(Letters.Persons.Where(x => x.Value.Index == index).First().Key + " Index laut val= " + person.Index);
            }
            return person.Name;
        }

        public static string GetPageXisOn(XElement lineElement, LetterObj letter)
        {
            string page = "";
            if (lineElement.ElementsBeforeSelf().Count() == 0 || lineElement.ElementsBeforeSelf().LastOrDefault(x => x.Name.LocalName == "page") == null)
            {
                // TODO: Only if ZH != Null
                page = letter.Meta.ZH.Page;
            }
            else
            {
                var pageTag = lineElement.ElementsBeforeSelf()?.Last(x => x.Name.LocalName == "page");
                page = pageTag.Attribute("autopsic").Value;
            }
            return page;
        }

        public static string GetPageAndLine(XElement editTag, LetterObj letter)
        {
            string line = "";
            var lineTag = GetLineXisIn(editTag);
            line = lineTag.Attribute("autopsic").Value;

            string page = GetPageXisOn(lineTag, letter);
            return (page + ", " + line + ": ");
        }

        static public Comment GetComment(XElement xelem)
        {
            //ref und subref id beziehen
            string refer = xelem.Attribute("ref").Value;
            string subref = xelem?.Attribute("subref")?.Value;
            Comment comm;
            //checken ob der ref als key in der kommentarliste existier
            if (Letters.Comments.ContainsKey(refer))
            {
                comm = Letters.Comments[refer];
                if (subref != null)
                {
                    comm = GetSubcomment(comm, subref);
                }
            }
            else
            {
                if (subref != null && Letters.Comments.ContainsKey(subref))
                {
                    comm = Letters.Comments[subref];
                }
                else
                {
                    Logger.Out("refer " + refer + "\nund subref " + subref + " existiert nicht als key in den comments.");
                    return null;
                }
            }
            return comm;
        }

        public static Comment GetSubcomment(Comment comm, string subref)
        {
            Comment subcomm;
            if (comm.Kommentare.ContainsKey(subref))
            {
                subcomm = comm.Kommentare[subref];
            }
            else
            {
                Logger.Out("subref " + subref + " existiert nicht als key in comments.");
                return null;
            }
            return subcomm;
        }

        public static bool CheckIfTextnode(XElement xelem)
        {
            bool truth = false;
            string str = "";
            XNode xnode = xelem as XNode;
            var prev = xnode.PreviousNode;
            if (prev != null)
            {
                if (prev is XText)
                {
                    str = (prev as XText).Value;
                    Regex regex = new Regex("[Vv]gl\\.[ <]");
                    Match match = regex.Match(str);
                    return match.Success;
                }
            }
            return truth;
        }

        public static bool CheckIfIntlinkBefore(XElement xelem)
        {
            bool truth = false;
            if (xelem.ElementsBeforeSelf().Any() && xelem.ElementsBeforeSelf().Last().Name.LocalName == "intlink")
            {
                truth = true;
            }
            return truth;
        }

        public static string RemoveWhiteSpaceEditkomm(string str)
        {
            Regex regex = new Regex("(<\\s?editreason\\s?index\\s?=\\s?\"[0-9]*\"\\s?>)\\s+");
            Match match = regex.Match(str);
            if (match.Success)
            {
                return regex.Replace(str, "$1");
            }
            else
            {
                return str;
            }
        }
        public static string RemoveWhiteSpaceLinebreak(string str)
        {
            Regex regex = new Regex("(<line type=\\s?\"break\"\\s?/>)\\s*");
            Match match = regex.Match(str);
            if (match.Success)
            {
                return regex.Replace(str, "$1");
            }
            else
            {
                return str;
            }
        }

        public static string RemoveEdits(string str)
        {
            Regex openEdit = new Regex("<edit\\s?ref=\"[0-9]*\">");
            Match openMatch = openEdit.Match(str);
            Regex closeEdit = new Regex("</edit>");
            Match closeMatch = openEdit.Match(str);
            if (closeMatch.Success && openMatch.Success)
            {
                str = closeEdit.Replace(str, "");
                str = openEdit.Replace(str, "");
            }
            return str;
        }

        public static bool CheckForContent(Paragraph para)
        {
            string text = para.Elements<Run>().Aggregate("", (ttext, r) => ttext += r.InnerText);
            if (text.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CheckLineTag(XElement xelem, WordprocessingDocument wordDoc)
        {

            bool isFootNote = false;
            Paragraph newOne = null;

            if (xelem.Ancestors().Any() && xelem.Ancestors().Where(x => x.Name.LocalName == "fn").Any())
            {
                isFootNote = true;
            }

            if (xelem.Attribute("index") != null)
            {
                //coutable: Zeile ist eine der zu zählenden 5er Zeilen
                bool isCountable = CheckIndex(xelem);
                bool isTabulated = CheckIfTab(xelem);
                if (isCountable & isTabulated)
                {
                    IfLine5(wordDoc, xelem, Int32.Parse(xelem.Attribute("index").Value));
                    IfTab(wordDoc, xelem, isFootNote);
                    return;
                }
                else if (isTabulated)
                {
                    IfTab(wordDoc, xelem, isFootNote);
                    return;

                }
                else if (isCountable)
                {
                    Paragraph counter = IfLine5(wordDoc, xelem, Int32.Parse(xelem.Attribute("index").Value));
                    newOne = counter.InsertAfterSelf<Paragraph>(new Paragraph());
                }
                else
                {
                    Paragraph lastParagraph = GetLastPara(wordDoc);
                    newOne = lastParagraph.InsertAfterSelf<Paragraph>(new Paragraph());
                }
            }
            else
            {
                Paragraph lastParagraph = GetLastPara(wordDoc);
                newOne = lastParagraph.InsertAfterSelf<Paragraph>(new Paragraph());
            }

            if (newOne != null)
            {
                if (isFootNote)
                {
                    ApplyParaStyle(newOne, "fußnote");
                }
                else
                {
                    ApplyParaStyle(newOne, "stumpf");
                    CheckLineType(xelem, newOne);
                }
            }
            else { Logger.Out("bei\n" + xelem + "\nist der neue Para 0"); }
        }

        public static void CheckLineType(XElement xelem, Paragraph para)
        {
            if (xelem.Attribute("type") != null)
            {
                if (xelem.Attribute("type").Value == "twoStars")
                {
                    para.AppendChild(new Run(new Text("**")));
                    para.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });
                }
                if (xelem.Attribute("type").Value == "threeStars")
                {
                    para.AppendChild(new Run(new Text("***")));
                    para.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });
                }
            }
        }

        public static bool CheckIndex(XElement xelem)
        {
            if (xelem.Attribute("index") != null)
            {
                int number = 0;
                if (Int32.TryParse(xelem.Attribute("index").Value, out number)) 
                {
                    if (number % 5 == 0 && number != 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool CheckIfTab(XElement xelem)
        {
            if (xelem.Attribute("tab") != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void IfTab(WordprocessingDocument wordDoc, XElement xelem, bool fn = false)
        {
            //stellt die korrekten einzüge für tab/fn Zeilen her
            Paragraph lastParagraph = GetLastPara(wordDoc);
            int tabval = Int32.Parse(xelem.Attribute("tab").Value);
            Paragraph newLastParagraph = lastParagraph.InsertAfterSelf<Paragraph>(new Paragraph());
            ApplyParaStyle(newLastParagraph, "stumpf");
            CheckLineType(xelem, newLastParagraph);
            lastParagraph = newLastParagraph;
            switch (tabval)
            {
                case 1:
                    if (fn == false)
                    {
                        ApplyParaStyle(lastParagraph, "einzug");
                    }
                    if (fn == true)
                    {
                        ApplyParaStyle(lastParagraph, "fußnote");
                        lastParagraph.ParagraphProperties.AppendChild(new Indentation() { Left = MiddleLineHight });
                    }
                    break;
                case 2:
                    ApplyParaStyle(lastParagraph, "doppeleinzug");
                    break;
                case 3:
                    ApplyParaStyle(lastParagraph, "dreifacheinzug");
                    break;
                case 4:
                    ApplyParaStyle(lastParagraph, "vierfacheinzug");
                    break;
                case 5:
                    ApplyParaStyle(lastParagraph, "fünffacheinzug");
                    break;
                case 6:
                    ApplyParaStyle(lastParagraph, "sechsfacheinzug");
                    break;
                case 7:
                    ApplyParaStyle(lastParagraph, "siebenfacheinzug");
                    break;
                default:
                    ApplyParaStyle(lastParagraph, "einzug");
                    break;
            }
        }

        public static void CheckPageTag(XElement xelem, WordprocessingDocument wordDoc)
        {
            //erstellt Textboxen für die Seitenzählung
            string pagenumber = "";
            if (xelem.Attributes("autopsic").Any())
                pagenumber = xelem.Attribute("autopsic").Value.ToString();
            else if (xelem.Attributes("index").Any())
                pagenumber = xelem.Attribute("index").Value.ToString();
            Paragraph counterParagraph = new Paragraph();
            ApplyParaStyle(counterParagraph, "seitenzählung");
            Run run = new Run(new Text("S. " + pagenumber));
            counterParagraph.AppendChild<Run>(run);
            SmallFont(run);
            BoldRun(run);
            Paragraph lastParagraph = GetLastPara(wordDoc);
            lastParagraph.InsertAfterSelf<Paragraph>(counterParagraph);
            FrameCounterParagraph(counterParagraph);
        }

        public static Paragraph IfLine5(WordprocessingDocument wordDoc, XElement xelem, int numbr)
        {
            Paragraph counterParagraph = new Paragraph();
            Run run = new Run(new Text(xelem.Attribute("autopsic").Value));
            counterParagraph.AppendChild<Run>(run);
            SmallFont(run);
            ApplyParaStyle(counterParagraph, "zeilenzählung");
            Paragraph lastParagraph = GetLastPara(wordDoc);
            lastParagraph.InsertAfterSelf<Paragraph>(counterParagraph);
            FrameCounterParagraph(counterParagraph);
            return counterParagraph;
        }

        public static XElement StringToXElement(string s)
        {
            return XElement.Parse(s, LoadOptions.PreserveWhitespace);
        }

        public static Run MakeTextRun(XNode node = null)
        {//erzeugt einen run der je nach parameter leer sein kann oder text enhält
            Run run = new Run();
            run.PrependChild(new RunProperties());
            if (node != null)
            {
                var xtext = (node as XText);
                string text = xtext.Value;
                run.AppendChild(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
            }
            return run;
        }

        public static Run MakeRun(Paragraph para, XNode xnode)
        {
            /*erzeugt einen run aus einer textnode als xelement und fügt ihn dem paragraph an*/
            var xtext = xnode as XText;
            string text = xtext.Value;
            Run run = new Run();
            RunProperties runProperties = run.PrependChild(new RunProperties());
            run.AppendChild(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
            Paragraph para2 = para.Parent.Elements<Paragraph>().Last<Paragraph>();
            para2.AppendChild(run);
            return run;
        }


        public static void MakeTradition(LetterObj letter)
        {
            //fügt den absatz mit der überlieferung an das dokument an
            var lastPara = GetLastPara(letter.WordDoc);
            Paragraph traditonPara = new Paragraph(new Run());
            lastPara.InsertAfterSelf<Paragraph>(traditonPara);
            CreateTraditionPara(letter, traditonPara);
            // CreateColSection(letter.WordDoc, true);
        }

        public static Paragraph CreateTraditionPara(LetterObj letter, Paragraph tradPara)
        {
            if (letter?.Tradition != null)
            {
                ApplyParaStyle(tradPara, "überlieferung");
                var tradString = letter.Tradition.ToString();
                XElement tradition = StringToXElement(RemoveWhiteSpaceLinebreak(tradString));
                ParseTraditionXElement(tradition, tradPara, letter);
            }
            return tradPara;
        }

        public static void AddRefsToDict(string refer, string subref, Dictionary<string, Dictionary<string, List<Place>>> commRefs, Place place)
        {
            //übreprüft ob ref-/subref-ids schon im dictionary enthalten sind und fügt diese gegebenenfalls sammt Stelle des Kommentars hinzu
            if (commRefs.ContainsKey(refer))
            {
                if (commRefs[refer].ContainsKey(subref))
                {
                    commRefs[refer][subref].Add(place);
                }
                else
                {
                    var list = new List<Place> { place };
                    commRefs[refer].Add(subref, list);
                }
            }
            else
            {
                var dict = new Dictionary<string, List<Place>>();
                var list = new List<Place> { place };

                if (subref == "0")
                {
                    dict.Add("0", list);
                }
                else
                {
                    dict.Add(subref, list);
                }
                commRefs.Add(refer, dict);
            }
        }

        public static int MatchIdSubstrings(string source)
        {
            // teilt die Bibelreferenz-ids in substrings und gibt diese zurück, überflüssiger behelf, ersetzt durch .sort atribut

            /*Regex stringParts = new Regex("([a-z]{1,4})-([a-z]{1,4})[0-9]{0,3}");
            Match stringMatch = stringParts.Match(source);
            string testament = stringMatch.Groups[1].Value;
            string exact = stringMatch.Groups[2].Value;
            Regex bookNumberPart = new Regex(".*-.*([0-9]{1,3})");
            Match bookNumberMatch = bookNumberPart.Match(source);
            string bookNumber = "0";
            if (bookNumberMatch.Success)
            {
                bookNumber = bookNumberMatch.Groups[1].Value;
            }
            Regex 
            
            berPart = new Regex(".*-.*-([0-9]{1,4})");
            Match lineNumberMatch = lineNumberPart.Match(source);
            string lineNumber = "0";
            if (lineNumberMatch.Success)
            {
                lineNumber = lineNumberMatch.Groups[1].Value;
            }
            return new string[] { testament, exact, bookNumber, lineNumber };*/
            Regex numberRegx = new Regex(@"(\d+)$");
            Match numberMatch = numberRegx.Match(source);
            string number = "0";
            if (numberMatch.Success)
            {
                number = numberMatch.Groups[1].Value;
            }
            return Int32.Parse(number);
        }

        public static string[] MatchForschungSubstrings(string source)
        {
            // teilt die Bibliographie ids in substrings und gibt diese zurück, überflüssiger behelf, ersetzt durch .sort atribut
            Regex stringParts = new Regex("([a-z]{3})-(.)-([0-9]{4})([a-z]?)");
            Match stringMatch = stringParts.Match(source);
            string name = stringMatch.Groups[1].Value;
            string letter = stringMatch.Groups[2].Value;
            string year = stringMatch.Groups[3].Value;
            string numberInYear = "-";
            if (stringMatch.Groups[4].Value.Length != 0)
            {
                numberInYear = stringMatch.Groups[4].Value;
            }
            return new string[] { source, name, letter, year, numberInYear };
        }

        public static void CorrectLinks(string comment, bool test = false)
        {
            /*da das hinzufügen von Links das öffnen des maindocument parts erfordert, 
             * dieses aber zu IO-Errors führt, werden die Links nach erstellen des 
             * Dokuments (bei nochmaligem öffnen und schließen) erzeugt 
             KEINE TOLLE LÖSUNG, funktioniert aber gut*/

            string wordPath = VolumesOutputDir + "HKB_" + comment + ".docx";
            if (test) { wordPath = comment; };
            using (WordprocessingDocument doc = WordprocessingDocument.Open(wordPath, true))
            {
                MainDocumentPart mainPart = doc.MainDocumentPart;
                foreach (Hyperlink link in mainPart.Document.Body.Descendants<Hyperlink>())
                {
                    if (link.Id == null || link.Id == "")
                    {

                        Logger.Out("defekter Link bei " + link.InnerText);
                    }
                    else
                    {
                        string url = link.Id;
                        try
                        {
                            var rel = mainPart.AddHyperlinkRelationship(new Uri(url), true);
                            link.Id = rel.Id;
                        }
                        catch (Exception exception)
                        {
                            Logger.Out("Diese URL ist im Eimer: " + url+ "\nObwohl ich sie lösche mag Word das Dokument danach nicht öffnen! Diese URL also bitte im XML korrigieren! \n"+exception.Message);
                            link.Remove();
                        }
                    }
                }
                mainPart.Document.Save();
                doc.Save();
                doc.Close();
            }
        }

        public static string GetTemporaryDirectory()
        {
            //erzeugt temporäres dir für das herstellen der Banddokumente, das später wieder gelöscht wird.
            //die temporären zh-Briefdateien werden dort bis zum mergen gespeichert
            string tempFolder = Path.GetTempFileName();
            try
            {
                File.Delete(tempFolder);
                Directory.CreateDirectory(tempFolder);
            }
            catch (Exception)
            {
                Logger.Out("Temporäres Verzeichnis " + tempFolder + " für die speicherung temporärer briefdokumente konnte nicht erstellt werden.");
            }

            return tempFolder;
        }

        public static void RemoveTempFolderFiles(Dictionary<int, string> outputPaths, string tempfolder)
        {
            foreach (var path in outputPaths.Where(x => x.Key != 0 && x.Key != 1000000000))
            {
                try
                {
                    File.Delete(path.Value);
                }
                catch (Exception)
                {
                    Logger.Out("löschen der temporären Briefdatei " + path + "ist fehlgeschlagen.\n Ist keine katastrophe, sollte aber manuell nachgeholt werden.");
                }
            }
            try
            {
                Directory.Delete(tempfolder);
            }
            catch (Exception)
            {
                Logger.Out("löschen des temporären Verzeichnisses " + tempfolder + "ist fehlgeschlagen.\n Ist keine katastrophe, sollte aber manuell nachgeholt werden.");
                Environment.Exit(0);
            }
        }

        public static void MergeDocx(string year, Dictionary<int, string> outputPaths)
        {
            /*verbindet die einzelnen temporären ZH Briefdateien zu einer Banddatei
             die dauerhaften Brief.docx dateien können nicht gemergt werden, da sie die 
             für Bände überflüssige source section am ende enthalten und alt chunks nicht 
             verändert werden können*/
            Logger.Out("erstelle Jahr " + year);
            var OrderedPathKeys = outputPaths.Keys.ToList();
            OrderedPathKeys.Sort((a, b) => -1 * a.CompareTo(b));
            foreach (var key in OrderedPathKeys.Where(k => k != 0))
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Open(@outputPaths[0], true))
                {
                    MainDocumentPart mainPart = doc.MainDocumentPart;
                    string altChunkId = "AltChunkId" + key;
                    AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                    using (FileStream fileStream = File.Open(@outputPaths[key], FileMode.Open))
                    {
                        chunk.FeedData(fileStream);
                    }
                    AltChunk altChunk = new AltChunk();
                    altChunk.Id = altChunkId;
                    mainPart.Document.Body.InsertAfter(altChunk, GetLastPara(doc));
                    mainPart.Document.Save();
                    doc.Close();
                }
            }
        }
    }
}
