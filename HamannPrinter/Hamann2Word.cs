using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HaDocument.Interfaces;
using HaDocument.Models;
using System.Threading.Tasks;
using System.Diagnostics;
using static HamannPrinter.Parser;
using Comment = HaDocument.Models.Comment;
using static HamannPrinter.Helper;

namespace HamannPrinter
{
    public class Hamann2Word
    {
        //Daten für das Parsen. Zum einen Verzeichnis für die Ausgabe und erstellte ILibrary:
        static public ILibrary Letters { get; set; }
        static public string OutputDir { get; set; }
        static public string VolumesOutputDir { get; set; }
        static public string Editionsrichtlinien { get; set; }

        //Zum anderen Typographische Maße für das erstellen der Datein:
        //Standartschriftgröße
        static public string FontSize { get; set; }
        //standart Zeilenhöhe
        static public string LineHight { get; set; }
        //wird zum trennen verschiedener Bereiche benutzt
        static public string SmallDistance { get; set; }
        //Mittlere Zeilenhöhe, wird hauptsächlich für Abstände zwischen Absätzen benutzt
        static public string MiddleLineHight { get; set; }
        //oberer Rand
        static public int MarginTop { get; set; }
        //rechter Rand
        static public UInt32 MarginRight { get; set; }
        static public UInt32 MarginRightColumns { get; set; }
        //unterer Rand
        static public int MarginBottom { get; set; }
        // linker Rand
        static public UInt32 MarginLeft { get; set; }
        // Abstand zwischen Footer und Rand
        static public UInt32 MarginFooter { get; set; }
        //Abstand zwischen Kolumnen
        static public string ColumnDistance { get; set; }
        //kleine Schriftgröße
        static public string SmallFontSize { get; set; }
        //Mittlere Schriftgröße
        public static string MiddleFontSize { get; set; }
        //Große Schrift
        static public string BigFontSize { get; set; }
        // Abstand zwischen Footer und Text
        static public string FooterToText { get; set; }
        //name der Standartfont (serifen)
        static public string NormalFont { get; set; }
        // Name der alternativen Font (serifenlos)
        static public string SpecialFont { get; set; }
        static public string Diodone { get; set; }
        //Positionswert von Hochstellungen
        static public string SuperValue { get; set; }
        //Positionswert von Tiefstellungen
        static public string SubValue { get; set; }
        // Pfad für die Ausgabe der Register
        static public List<string> RegisterPaths { get; set; }
        // Pfad für die Ausgabe der Volumenbände
        static public List<string> YearPaths { get; set; }
        // Pfad für Ausgabe der Briefe
        static public List<string> LetterPaths { get; set; }

        public Hamann2Word(ILibrary lib, DocOptions docxOptions)
        {
            // erzeugt neue Instanz von Hamann2Word mit den Erforderlichen Werten aus den DocOptions
            Letters = lib;
            OutputDir = docxOptions.OutputDir;
            VolumesOutputDir = docxOptions.VolumesOutputDir;
            Editionsrichtlinien = docxOptions.Editionsrichtlinien;
            FontSize = docxOptions.FontSize;
            MiddleFontSize = docxOptions.MiddleFontSize;
            LineHight = docxOptions.LineHight;
            SmallDistance = docxOptions.SmallDistance;
            MiddleLineHight = docxOptions.MiddleLineHight;
            MarginLeft = docxOptions.MarginLeft;
            MarginRight = docxOptions.MarginRight;
            MarginTop = docxOptions.MarginTop;
            MarginBottom = docxOptions.MarginBottom;
            MarginFooter = docxOptions.MarginFooter;
            ColumnDistance = docxOptions.ColumnDistance;
            SmallFontSize = docxOptions.SmallFontSize;
            BigFontSize = docxOptions.BigFontSize;
            FooterToText = docxOptions.FooterToText;
            NormalFont = docxOptions.NormalFont;
            SpecialFont = docxOptions.SpecialFont;
            SuperValue = docxOptions.SuperValue;
            SubValue = docxOptions.SubValue;
            Diodone = docxOptions.Diodone;
            MarginRightColumns = docxOptions.MarginRightColumns;
            RegisterPaths = new List<string>();
            YearPaths = new List<string>();
            LetterPaths = new List<string>();
        }

        public class Place
        {
            /*Klasse erzeugt Objekte die Band, Brief, Seite und Zeile einer Kommentarreferenz enthalten
             dies vereinfacht das Sortieren der Belegstellen für die Registerbände*/
            public int Volume { get; set; } = 0;
            public string Letter { get; set; }
            public int Page { get; set; }

            public string PageString { get; set; }
            public int Line { get; set; }
            public Place(int volume, string letter, int page, int line, string pgstring)
            {
                Volume = volume;
                Letter = letter;
                Page = page;
                Line = line;
                PageString = pgstring;
            }
        }

        public class LetterObj
        {
            //Rerpäsentiert einen HamannBrief

            //Verzeichnis der docx. Datei des Briefes
            public string OutPutFile { get; set; }
            //Programminterner Index des Briefes
            public string Key { get; set; }
            //Index des Briefes
            public string Index { get; set; }
            //autopsic index des briefes
            public string Autopsic { get; set; }
            // Text des Briefes als XElement
            public XElement Text { get; set; }
            // Überlieferung des Briefes als XEelement
            public XElement Tradition { get; set; }
            //Metainformationen des Briefes
            public Meta Meta { get; set; }
            //Liste der Editorischen Eingriffe in den Brief als Referenz-Id aus dem XML
            public List<string> Edits { get; set; }
            //Liste der <hand> elemente im Brief als XElement
            public List<XElement> HandTags { get; set; }
            //docx Datei des Briefes
            public WordprocessingDocument WordDoc { get; set; }
            public bool stateFirstLine { get; set; } = true;

            public LetterObj(string index, XElement text, Meta meta, string key, string tempDir = null)
            {
                HandTags = new List<XElement>();
                Edits = new List<string>();
                Index = index;
                Meta = meta;
                Autopsic = Meta.Autopsic;
                Text = text;
                Key = key;
                GetOutPutFile(tempDir);
            }

            private void GetOutPutFile(string tempDir = null)
            {
                // var band = this.Meta.ZH.Volume;
                var autopsic = this.Autopsic;
                string outfile = OutputDir +  "HKB_" + autopsic + ".docx";
                if (tempDir != null)
                {
                    outfile = tempDir + "HKB_" + autopsic + ".docx";
                }
                this.OutPutFile = outfile;
            }
        }


        public static void Letter2Docx(LetterObj letter)
        {
            /*befüllt ein LetterObj.BriefDocx mit dem "geparsten" Inhalt (xml) des Briefes*/

            Logger.Out("Brief: " + letter.Autopsic);
            Paragraph para = GetLastPara(letter.WordDoc);
            var txtElem = letter.Text;

            //loopt über die Nodes des BriefXmls 
            foreach (XNode xnode in txtElem.Nodes())
            {
                /*wenn die node der ersten ebene ein XElelemnt ist, werden die 
                 * ihrem typ entsprechenden formatierungs informationen auf den "StyleStack" gelegt.
                 dann wird mit WalkNodeTree über die ChildNodes geloopt*/
                if (xnode is XElement)
                {
                    XElement xelem = xnode as XElement;
                    Formatierer stack = null;
                    stack = ProcessXelement(stack, xelem, letter.WordDoc, letter);
                    if (xelem.LastNode != null)
                    {
                        WalkNodeTree(xelem.LastNode, stack, para, letter.WordDoc, letter);
                    }
                }
                /*wenn node der ersten ebene XText ist, wird ihr Textinhalt als Run dem letzten Absatz des Dokuments angehängt*/
                else if (xnode is XText)
                {
                    Paragraph lastParagraph = GetLastPara(letter.WordDoc);
                    MakeRun(lastParagraph, xnode);
                }
            }
            //Kommentare und Varianten anhängen
            AddCommentsETC(letter);
        }


        public static void AddCommentsETC(LetterObj letter)
        {
            if (letter.Tradition != null)
            {
                MakeTradition(letter);
            }

            //checken ob marginals, hands und editreasons nötig sind
            bool edits = false;
            bool editSingle = false;
            bool margs = false;
            bool margSingle = false;
            bool hands = false;
            bool handSingle = false;

            if (Letters.EditreasonsByLetter[letter.Index].Any())
            {
                edits = true;
                if (Letters.EditreasonsByLetter[letter.Index].Count() == 1)
                {
                    editSingle = true;
                }
            }

            if (letter.HandTags.Count() != 0)
            {
                hands = true;
                if (letter.HandTags.Count() < 3)
                {
                    handSingle = true;
                }
            }

            if (Letters.MarginalsByLetter[letter.Index].Any())
            {
                margs = true;
                if (Letters.MarginalsByLetter[letter.Index].Count() == 1)
                {
                    margSingle = true;
                }
            }

            if (hands)
            {
                //XColHeading(letter.WordDoc, "Zusätze von fremder Hand", 1);
                CreateHandComments(letter);
                CreateColSection(letter.WordDoc, handSingle);

            }

            if (edits)
            {
                //XColHeading(letter.WordDoc, "Textkritische Anmerkungen", 1);
                MakeEdits(letter);
                CreateColSection(letter.WordDoc, editSingle);
            }


            if (margs)
            {
                //XColHeading(letter.WordDoc, "Kommentar", 1);
                MakeComms(letter);
                CreateColSection(letter.WordDoc, margSingle);
            }

            MakeFinalPageBreak(letter);
        }

        public static void MakeSourceSection(LetterObj letter)
        {
            string txt = "Johann Georg Hamann: Kommentierte Briefausgabe (HKB). Hrsg. von Leonard Keidel und Janina Reibold, auf Grundlage der Vorarbeiten Arthur Henkels, unter Mitarbeit von Gregor Babelotzky, Konrad Bucher, Christian Großmann, Carl Friedrich Haak, Luca Klopfer, Johannes Knüchel, Isabel Langkabel und Simon Martens. (Heidelberg 2020ff.) URL: ";
            var link = new Run(new Text("www.hamann-ausgabe.de"));
            var head = new Run(new Text("Quelle:"));
            var src = new Run(new Break(), new Text(txt) { Space = SpaceProcessingModeValues.Preserve });
            var srcPara = new Paragraph(head, src, link, new Run(new Text(".")));
            ApplyParaStyle(srcPara, "quelle");
            GetLastPara(letter.WordDoc).Remove();
            GetLastPara(letter.WordDoc).InsertAfterSelf(srcPara);
        }


        public static void MakeEdits(LetterObj letter)
        {
            /*List<Editreason> letterEdits = Letters.EditreasonsByLetter[letter.Key].ToList();
            var OrderEdits = letterEdits.OrderBy(m => m.Index);
            foreach (var edit in OrderEdits)*/
            MakeHeading(letter.WordDoc, "Textkritische Anmerkungen");
            for (int c = 0; c < letter.Edits.Count();)
            {
                if (Letters.Editreasons.ContainsKey(letter.Edits[c]))
                {
                    Editreason edit = Letters.Editreasons[letter.Edits[c]];
                    Paragraph lastPara = GetLastPara(letter.WordDoc);
                    Paragraph lemmaPara = MakeLemmaPara(lastPara, edit, letter);
                    MakeEditorPara(lemmaPara, edit);
                }
                else
                {
                    Logger.Out("Editreferenz " + letter.Edits[c] + " fehlt!");
                }
                c++;
            }
        }

        public static Paragraph MakeLemmaPara(Paragraph lastPara, Editreason edit, LetterObj letter)
        {
            Paragraph editLemma = new Paragraph();
            lastPara.InsertAfterSelf(editLemma);
            ApplyParaStyle(editLemma, "textkritik");
            var pageRun = editLemma.AppendChild(new Run());
            SansSerifRun(pageRun);
            pageRun.AppendChild(new Text(edit.StartPage + "/" + edit.StartLine + " ") { Space = SpaceProcessingModeValues.Preserve });
            SmallFont(pageRun, MiddleFontSize);
            BoldRun(pageRun);
            if (edit.Reference != null)
            {
                string editRefString = RemoveWhiteSpaceLinebreak(edit.Reference);
                XElement editReference = StringToXElement(editRefString);
                ParseSublementaXElement(editReference, editLemma);
                editLemma.AppendChild<Run>(new Run(new Text("] ") { Space = SpaceProcessingModeValues.Preserve }));
            }
            else
            {
                Logger.Out(edit.Letter + " \n\n\n\n\n\nHier gibt es keine Edit-Referenz " + edit.Element);
            }
            return editLemma;
        }


        public static void MakeEditorPara(Paragraph editLemma, Editreason edit)
        {
            string editText = RemoveWhiteSpaceEditkomm(edit.Element);
            editText = RemoveWhiteSpaceLinebreak(editText);
            editText = RemoveEdits(editText);
            XElement editComment = StringToXElement(editText);
            if (editComment != null)
            {
                ParseSublementaXElement(editComment, editLemma);
            }
            else
            {
                Logger.Out("editComment ist null:\n" + editText);
            }
        }

        public static void WalkNodeTree(XNode xnode, Formatierer stack, Paragraph para, WordprocessingDocument wordDoc, LetterObj letter)
        {
            /*komische, rekursive Lösung, um durch den Node-Tree des XMl zu klettern, 
            die leaf nodes vom type xtext zu identifizieren, entsprechend der sie enthaltenden 
            elemente zu formatieren und anschließend in dateireihenfolge an dan dokument anzufügen.
            Diese Funtkion ist ein schönes Beispiel dafür, was passiert, wenn man etwas tut, das
            man nicht versteht, funtkioniert aber "gut".*/
            while (xnode != null)
            {
                //wenn die node nicht die erste node auf der ebene ist, verarbeite zuerst die vor ihr liegende
                if (xnode.PreviousNode != null)
                {
                    WalkNodeTree(xnode.PreviousNode, stack, para, wordDoc, letter);
                }

                /*wenn die node ein XElement ist, wird ihr Formatierungswert auf den Stack gelegt, 
                und mit dem Parsen ihrer letzten childnode rekursiv begonnen (durch die while-Schleife)*/
                if (xnode is XElement)
                {
                    XElement xelem = xnode as XElement;
                    stack = ProcessXelement(stack, xelem, wordDoc, letter);
                    xnode = xelem.LastNode;
                }

                /*wenn die node xtext ist, wird der Text mittels des Formatierungsstacks formatiert und dem dokument angehängt.*/
                else if (xnode is XText)
                {
                    Paragraph lastParagraph = GetLastPara(wordDoc);
                    Run run = MakeRun(lastParagraph, xnode);
                    stack?.Invoke(run);
                    xnode = null;
                }
            }
        }

        public static WordprocessingDocument CreateLetterDocx(string path)
        {
            /*Erzeugt leeres, unformatierstes Docx. für Briefe*/
            WordprocessingDocument WordDoc = null;
            int? c = 0;
            while (c != null)
            {
                // falls die zu erstllende Datei geöffent ist ... 
                try
                {
                    WordDoc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document);
                    c = null;
                }
                catch (IOException)
                {
                    Warn("Die Datei " + path + " kann nicht erstellt werden. Höchstwahrscheinlich ist sie geöffnet.\nBitte schließen und Ok drücken.");
                    c++;
                    if (c > 2)
                    {
                        Warn("Die Datei " + path + " kann nicht erstellt werden. \nFalls es nicht daran liegt, dass sie geöffnet ist, ist mir die Sache ein Rätsel.\nProgramm wird beendet!\n\nGrüße, Fritz");
                        Environment.Exit(0);
                    }
                }
            }
            LetterPaths.Add(path);
            return WordDoc;
        }

        public static WordprocessingDocument MakeEmptyDocx(string outfile)
        {
            //erzeugt leere Dokumente für Banddokumente und Registerdokumente
            WordprocessingDocument wordDoc = CreateOutputFile(outfile);
            var main = wordDoc.AddMainDocumentPart();
            main.Document = new Document();
            StyleDefinitionsPart part = wordDoc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
            Styles root = new Styles();
            root.Save(part);
            CreateStyles(wordDoc);


            //create body
            Body body = main.Document.AppendChild(new Body());
            body.AppendChild(new Paragraph(new Run()));

            wordDoc.Save();
            return wordDoc;
        }


        public static void ProcessLink(XElement xelem, Paragraph para, int counter = 0)
        {
            if (xelem.Attributes("linktext").Count() != 0 && xelem.Attribute("linktext").Value == "true")
            {
                var comment = GetComment(xelem);
                if (comment == null)
                {
                    Logger.Out("für \n" + xelem.ToString() + "\n ist etwas schiefgegangen!");
                }
                else
                {
                    if (comment.Entry.Length > 0)
                    {
                        counter++;
                        if (counter < 1)
                        {
                            var xLemma = StringToXElement(comment.Lemma + " ");
                            ParseComment(xLemma, para, counter);
                            var xComment = StringToXElement(comment.Entry);
                            ParseComment(xComment, para, counter);
                        }
                        else
                        {
                            var xComment = StringToXElement(comment.Lemma);
                            ParseComment(xComment, para, counter);
                        }

                    }
                    else
                    {
                        Logger.Out("der Comment:\n" + comment.Entry + "\n für \n" + xelem + "\n ist leer");
                    }
                }
            }
        }

        private static void ProcessIntLink(XElement xelem, Paragraph para)
        {
            //< intlink letter = "7" page = "17" line = "27" linktext = "true" />
            string vol = "";
            if (xelem?.Attribute("linktext")?.Value == "true")
            {
                string letter = xelem?.Attribute("letter")?.Value;
                if (Letters.Metas.ContainsKey(letter))
                {
                    if (Letters.Metas[letter].ZH != null)
                        vol = 
                            //"ZH\u202F" + 
                            GetRoman(Letters.Metas[letter].ZH.Volume) + " ";
                }
                else
                {
                    Logger.Out("\n\n###########################\nKeine Metas für Nummer " + letter + "\naus\n" + xelem.ToString() + "\ngefunden\ndie existieren erst, wenn der entsprechende band mit geladen und erstellt wird.");
                }
                string page = "\u202F" + xelem?.Attribute("page")?.Value + "/";
                string line = xelem?.Attribute("line")?.Value;
                string total = "HKB\u202F";
                total += letter + " (" + vol + page + line + ")";
                Run r = new Run(new Text(total));
                para.AppendChild(r);
            }
            else if (xelem?.Attribute("linktext")?.Value != "false")
            {
                Logger.Out("intlink \n" + xelem.ToString() + "\nhat keinen gültigen int-linktext-value definiert");
            }
        }

        private static Run CommLineAndPage(Marginal marg)
        {
            Run run = new Run(new Text(marg.Page + "/" + marg.Line) { Space = SpaceProcessingModeValues.Preserve });
            BoldRun(run);
            SmallFont(run, MiddleFontSize);
            return run;
        }

        private static void MakeComms(LetterObj letter)
        {
            //CreateCommHeading(letter);
            var letterMarginals = Letters.MarginalsByLetter[letter.Index].ToList();
            //var oderedMarginals = letterMarginals.OrderBy(m => m.Page).ThenBy(m => m.Line);
            var oderedMarginals = letterMarginals.OrderBy(m => ConvertNumber(m.Page)).ThenBy(m => int.Parse(m.Line)).ThenBy(m => m.Index).ToList();
            MakeHeading(letter.WordDoc, "Kommentar");
            foreach (var marg in oderedMarginals)
            {
                var lastPara = GetLastPara(letter.WordDoc);
                var linePage = lastPara.InsertAfterSelf(new Paragraph(CommLineAndPage(marg)));
                ApplyParaStyle(linePage, "kommentar");
                XElement xelem = StringToXElement(marg.Element);
                ParseComment(xelem, linePage);
            }
        }

        private static Dictionary<char, int> _romanMap = new Dictionary<char, int>
        {
            {'I', 1}, {'V', 5}, {'X', 10}, {'L', 50}, {'C', 100}, {'D', 500}, {'M', 1000}
        };

        public static int ConvertNumber(string text)
        {
            int res;
            if (int.TryParse(text, out res))
            {
                return res;
            }
            else
            {
                int totalValue = 0, prevValue = 0;
                foreach (var c in text)
                {
                    if (!_romanMap.ContainsKey(c))
                        return 0;
                    var crtValue = _romanMap[c];
                    totalValue += crtValue;
                    if (prevValue != 0 && prevValue < crtValue)
                    {
                        if (prevValue == 1 && (crtValue == 5 || crtValue == 10)
                            || prevValue == 10 && (crtValue == 50 || crtValue == 100)
                            || prevValue == 100 && (crtValue == 500 || crtValue == 1000))
                            totalValue -= 2 * prevValue;
                        else
                            return 0;
                    }
                    prevValue = crtValue;
                }
                return totalValue;
            }
        }

        public static void MakeHeading(WordprocessingDocument wordDoc, string title)
        {
            //erzeugt und formatiert die überschriften für editorische anmerkungen, kommentare und zusätze von fremder hand
            // MakeFramedEmptyLines(wordDoc);
            var run = new Run();
            run.AppendChild(new Text(title));
            BoldRun(run);
            var headingPara = new Paragraph(run);
            ApplyParaStyle(headingPara, "überlieferung");
            var LineHeight = Int32.Parse(LineHight);
            // FrameHeadingParagraph(headingPara);
            headingPara.ParagraphProperties.AppendChild(new SpacingBetweenLines() { After = (LineHeight / 2).ToString(), Before = LineHight });
            headingPara.ParagraphProperties.AppendChild(new KeepNext() { Val = true });
            headingPara.ParagraphProperties.AppendChild<SectionProperties>(new SectionProperties());
            // headingPara.ParagraphProperties.SectionProperties.AppendChild(new KeepNext() { Val = true });
            // headingPara.ParagraphProperties.SectionProperties.AppendChild(new Columns () { ColumnCount = 1 });
            headingPara.ParagraphProperties.SectionProperties.AppendChild<SectionType>(new SectionType() { Val = SectionMarkValues.Continuous });
            PageMargin pageMargin = new PageMargin() { Top = MarginTop, Right = MarginRight, Bottom = MarginBottom, Left = MarginLeft, Footer = MarginFooter };
            headingPara.ParagraphProperties.SectionProperties.PrependChild(pageMargin);
            GetLastPara(wordDoc).InsertAfterSelf(headingPara);
            // var nextPara = new Paragraph();
            // ApplyParaStyle(nextPara, "stumpf");
            // nextPara.ParagraphProperties.AppendChild(new SpacingBetweenLines() { After = LineHight });
            // nextPara.ParagraphProperties.AppendChild(new KeepNext() { Val = false });
            // nextPara.ParagraphProperties.AppendChild<SectionProperties>(new SectionProperties());
            // nextPara.ParagraphProperties.SectionProperties.AppendChild(new Columns () { ColumnCount = 2 });
            // nextPara.ParagraphProperties.SectionProperties.AppendChild<SectionType>(new SectionType() { Val = SectionMarkValues.Continuous });
            // PageMargin pageMargin2 = new PageMargin() { Top = MarginTop, Right = MarginRight, Bottom = MarginBottom, Left = MarginLeft, Footer = MarginFooter };
            // nextPara.ParagraphProperties.SectionProperties.PrependChild(pageMargin2);
            // GetLastPara(wordDoc).InsertAfterSelf(nextPara);
        }

        public static Formatierer GetCommFormat(XNode xnode, Paragraph para = null, int counter = 0)
        {
            /*formatiert die runs in kommentaren entsprechend des xml, der counter 
             * dient dazu, intlinks nicht ewig "rekursiv" (passiert, weil die intlinks 
             * unendlich untereinander verlinkt sind) aufgelöst werden*/

            Formatierer form = null;
            XElement xelem = xnode as XElement;
            string tag = xelem.Name.LocalName;
            switch (tag)
            {
                case "line":
                    form = new Formatierer(NewLineRun);
                    break;
                case "intlink":
                    ProcessIntLink(xelem, para);
                    break;
                case "link":
                    ProcessLink(xelem, para, counter);
                    break;
                case "bzg":
                    form = new Formatierer(SerifRun);
                    form += new Formatierer(BracketAfterRun);
                    break;
                case "titel":
                    form = new Formatierer(IRun);
                    break;
                case "wwwlink":
                    var uri = @xelem.Attribute("address").Value;
                    if (uri == "" || uri == null)
                    {
                        Console.WriteLine(xelem.ToString());
                    }
                    else
                    {
                        form = new Formatierer(
                            delegate (Run run, string arg)
                            {
                                HyperLink(run, uri);
                            });
                    }
                    break;
            }
            return form;
        }

        public static void ParseComment(XElement list, Paragraph para, int counter = 0)
        {
            //parst einen kommentar, der counter dient dazu, dass die rekursion die über GetCommFormat eintreten kann nicht endlos ist
            if (list != null)
            {
                var singleNodes = list.DescendantNodes();
                foreach (var node in singleNodes)
                {
                    if (node is XElement && (node as XElement).IsEmpty)
                    {
                        Run run = MakeTextRun();
                        Formatierer stack = GetCommFormat(node, para, counter);
                        stack?.Invoke(run);
                        if (run.Parent != null)
                        {
                            para.AppendChild(run.Parent);
                        }
                        else
                        {
                            para.AppendChild(run);
                        }
                    }
                    else if (node is XText)
                    {
                        Run run = MakeTextRun(node);
                        Formatierer stack = null;
                        foreach (var anc in node.Ancestors())
                        {
                            stack += GetCommFormat(anc, para: para, counter);
                        }
                        stack?.Invoke(run);
                        if (run.Parent != null)
                        {
                            para.AppendChild(run.Parent);
                        }
                        else
                        {
                            para.AppendChild(run);
                        }
                    }
                }
            }
            else
            {
                Logger.Out("list was null!");
            }
        }

        public static Formatierer GetFormat(XNode xnode, Run run = null, Paragraph para = null)
        {   //wertet die atribute von xelementen aus, dient dazu, lemmata usw. zu formatieren(wertet den inhalt von xelementen aus)
            Formatierer form = null;
            XElement xelem = xnode as XElement;
            string tag = xelem.Name.LocalName;
            switch (tag)
            {
                case "letterTradition":
                    form = new Formatierer(SansSerifRun);
                    break;
                case "line":
                    string parent = GetHighestParentNode(xnode).Trim();
                    if (parent == "editreason")
                    {
                        form = new Formatierer(NewLineRun);
                    }
                    else if (parent == "edit" || parent == "letterText")
                    {
                        form = new Formatierer(LinebreakSlash);
                    }
                    else
                    {
                        form = new Formatierer(NewLineRun);
                    }
                    break;

                case "page":
                    if (run != null)
                    {
                        string pagenumber = xelem.Attribute("index").Value.ToString();
                        run.AppendChild<Text>(new Text(" |" + pagenumber + "| ") { Space = SpaceProcessingModeValues.Preserve });

                    }
                    break;

                case "app":
                case "emph":
                    form += new Formatierer(IRun);
                    break;

                case "nr":
                    form = new Formatierer(NonreadRun);
                    break;

                case "super":
                    form = new Formatierer(HighRun);
                    form += new Formatierer(SmallFont);
                    break;

                case "sub":
                    form = new Formatierer(LowRun);
                    form += new Formatierer(SmallFont);
                    break;

                case "aq":
                    form = new Formatierer(SansSerifRun);
                    break;

                case "i":
                    form = new Formatierer(IRun);
                    break;

                case "note":
                    form = new Formatierer(IRun);
                    form += new Formatierer(GreyRun);
                    break;

                case "ul":
                    form = new Formatierer(UlRun);
                    break;

                case "dul":
                    form = new Formatierer(DulRun);
                    break;

                case "tul":
                    form = new Formatierer(TrulRun);
                    break;

                case "del":
                    form = new Formatierer(StrikeRun);
                    break;

                case "added":
                    form = new Formatierer(GreyRun);
                    break;
            }


            return form;

        }


        public static Formatierer ProcessXelement(Formatierer stack, XElement xelem, WordprocessingDocument wordDoc, LetterObj letter)
        {
            /*dient zum formatieren des hauptrieftextes. wertet ein Xelement aus; 
             * meistens handelt es sich um formatierungen, die dann in Formatierer delegates 
             * verkettet werden um später Runs zu formatieren
             * erstellt listen mit den handtags und edits die später zum erstellen dieser 
             * bereiche verwendet werden*/

            Formatierer form = null;
            string tag = xelem.Name.LocalName;

            switch (tag)
            {
                case "letterTradition":
                    form = new Formatierer(SansSerifRun);
                    break;
                case "app":
                case "emph":
                    string emphParent = GetHighestParentNode(xelem).Trim();
                    if (emphParent == "letterTradition")
                    {
                        form = new Formatierer(MarginBefore);
                        form += new Formatierer(BoldRun);
                        form += new Formatierer(SansSerifRun);
                        form += new Formatierer(KeepNextRun);
                    }
                    else
                    {
                        form += new Formatierer(IRun);
                    }
                    break;

                case "line":
                    CheckLineTag(xelem, wordDoc, letter);
                    break;
                case "page":
                    if (!letter.stateFirstLine) 
                    {
                        CheckPageTag(xelem, wordDoc);
                    }
                    break;

                case "nr":
                    form = new Formatierer(NonreadRun);
                    break;

                case "super":
                    form = new Formatierer(HighRun);
                    form += new Formatierer(SmallFont);
                    break;

                case "sub":
                    form = new Formatierer(LowRun);
                    form += new Formatierer(SmallFont);
                    break;

                case "sal":
                    form = new Formatierer(SalutationRun);
                    break;

                case "aq":
                    form = new Formatierer(SansSerifRun);
                    break;

                case "note":
                    form = new Formatierer(IRun);
                    form += new Formatierer(GreyRun);
                    break;

                case "hand":
                    //die Liste HandTags wird später zur Referenzierung der <hand> Stellen gebraucht
                    letter.HandTags.Add(xelem);
                    form = new Formatierer(DiodoneRun);
                    break;

                case "ul":
                    form = new Formatierer(UlRun);
                    break;

                case "dul":
                    form = new Formatierer(DulRun);
                    break;

                case "tul":
                    form = new Formatierer(TrulRun);
                    break;

                case "edit":
                    string key = xelem.Attribute("ref").Value;
                    //die Liste HandTags wird später zur Referenzierung der <edit> Stellen gebraucht, auch wegen der Reihenfolge
                    letter.Edits.Add(key);
                    break;

                case "del":
                    form = new Formatierer(StrikeRun);
                    break;

                case "align":
                    if (xelem.Attribute("pos").Value == "right")
                    {
                        PosRight(wordDoc);
                    }
                    else if (xelem.Attribute("pos").Value == "center")
                    {
                        PosCenter(wordDoc);
                    }
                    break;

                case "added":
                    form = new Formatierer(GreyBackRun);
                    break;

                case "fn":
                    ProcessFootnoteXElement(xelem, letter);
                    break;
            }

            if (stack == null)
            {
                stack = form;
            }
            else
            {
                if (form != null)
                {
                    stack = form + stack;
                }
            }
            return stack;
        }


        public static void ProcessFootnoteXElement(XElement xelem, LetterObj letter)
        {
            //Testet ob die Fußnote die erste Fußnote unter dem Brief ist und ob die Fn eingezogen ist, formatiert dementsprechend die Absätze
            if (xelem.ElementsBeforeSelf().Any())
            {
                var last = GetLastPara(letter.WordDoc);
                var lastBefore = xelem.ElementsBeforeSelf().Last();
                string name = lastBefore.Name.LocalName;

                if (name != "fn")
                {
                    var heading = last.InsertAfterSelf(new Paragraph());
                    Run head = new Run(new Break(), new Text("Fußnoten:"));
                    IRun(head);
                    GreyRun(head);
                    heading.AppendChild(head);
                    ApplyParaStyle(heading, "fußnote");
                    heading.ParagraphProperties.PrependChild<KeepNext>(new KeepNext() { Val = true });
                    last = heading;
                }

                var fn = last.InsertAfterSelf(new Paragraph());
                ApplyParaStyle(fn, "fußnote");
                if (name == "line" && lastBefore.Attribute("tab") != null)
                {
                    fn.ParagraphProperties.PrependChild(new Indentation() { FirstLine = MiddleLineHight });
                }
            }
        }


        #region Vorbereitungen für Letterparsen


        public void MakeYearDocuments(ILibrary lib, (int, int) years)
        {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                // Parallel.ForEach(Enumerable.Range(years.Item1, years.Item2 - years.Item1 + 1), i => {
                //     Logger.Out("Dokument für Jahr " + i.ToString());
                //     var outputPaths = new List<(int, string)>();
                //     var letterList = Lib2List(lib, i);
                //     if (letterList.Any())
                //     {
                //         foreach (var letter in letterList.OrderBy(x => x.Meta.Sort).ThenBy(x => x.Meta.Order))
                //         {
                //             letter.WordDoc = CreateLetterDocx(letter.OutPutFile);
                //             StyleLetterDocx(letter);
                //             Letter2Docx(letter);
                //             outputPaths.Add((Int32.Parse(letter.Index), letter.OutPutFile));
                //             letter.WordDoc.Dispose();
                //         }
                //         MergeDocx(lib, i.ToString(), outputPaths, MakeVolumeDoc(i.ToString()), Editionsrichtlinien);
                //     }
                //     foreach(var letter in letterList) 
                //     {
                //         Logger.Out("Nachbearbeitung Brief " + letter.Autopsic);
                //         letter.WordDoc = WordprocessingDocument.Open(letter.OutPutFile, true);
                //         MakeSourceSection(letter);
                //         letter.WordDoc.Dispose();
                //     }
                // });
                // sw.Stop();
                // TimeSpan ts = sw.Elapsed;
                // string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                //     ts.Hours, ts.Minutes, ts.Seconds,
                //     ts.Milliseconds / 10);
                // Logger.Out(elapsedTime);
                // sw.Restart();
                for (; years.Item1 <= years.Item2; years.Item1++)
                {
                    Logger.Out("Dokument für Jahr " + years.Item1.ToString());
                    var outputPaths = new List<(int, string)>();
                    var letterList = Lib2List(lib, years.Item1);
                    if (letterList.Any())
                    {
                        foreach (var letter in letterList.OrderBy(x => x.Meta.Sort).ThenBy(x => x.Meta.Order))
                        {
                            letter.WordDoc = CreateLetterDocx(letter.OutPutFile);
                            StyleLetterDocx(letter);
                            Letter2Docx(letter);
                            outputPaths.Add((Int32.Parse(letter.Index), letter.OutPutFile));
                            letter.WordDoc.Dispose();
                        }
                        MergeDocx(lib, years.Item1.ToString(), outputPaths, MakeVolumeDoc(years.Item1.ToString()), Editionsrichtlinien);
                    }
                    Parallel.ForEach(letterList, letter => {
                        Logger.Out("Nachbearbeitung Brief " + letter.Autopsic);
                        letter.WordDoc = WordprocessingDocument.Open(letter.OutPutFile, true);
                        MakeSourceSection(letter);
                        letter.WordDoc.Dispose();
                    });
                        
                }
                sw.Stop();
                var ts = sw.Elapsed;
                var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Logger.Out("ELAPSED: " + elapsedTime);
        }

        public static void StyleLetterDocx(LetterObj letter)
        {
            //grundelgende Parts erzeugen und die Absatzvorlagen erstellen
            var wordDoc = letter.WordDoc;
            var main = wordDoc.AddMainDocumentPart();
            main.Document = new Document();
            StyleDefinitionsPart part = wordDoc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
            Styles root = new Styles();
            root.Save(part);
            CreateStyles(letter.WordDoc);

            //body erzeugen und mit den Metadaten-Absätzen erzeugen und anhängen
            Body body = main.Document.AppendChild(new Body());
            Paragraph nr = MakeLetterNrPara(letter);
            Paragraph zhPara = MakeZhPara(letter);
            Paragraph metaPara = MakeMetaPara(letter);
            body.AppendChild<Paragraph>(zhPara);
            body.AppendChild<Paragraph>(nr);
            body.AppendChild<Paragraph>(metaPara);


            //footer/sectionproperties erzeugen 
            Run footerun = CreateFooterRun(letter);
            SectionProperties sectionProperties = AppendFooter(main, footerun);


            //Seitenränder definieren
            PageMargin pageMargin = new PageMargin() { Top = MarginTop, Right = MarginRight, Bottom = MarginBottom, Left = MarginLeft, Footer = MarginFooter };
            sectionProperties.PrependChild(pageMargin);

            //Spaltenzahl für letzten Abschnitt definieren
            sectionProperties.PrependChild<Columns>(new Columns() { ColumnCount = 1 });
            sectionProperties.AppendChild<SectionType>(new SectionType() { Val = SectionMarkValues.Continuous });

            //sectionproperties anhängen
            main.Document.Body.PrependChild(sectionProperties);
        }

        public static List<LetterObj> Lib2List(ILibrary library, (int, int) years)
        {
            /*Erstellt eine Liste von LetterObj. des Entpsrechenden 
             * Bandes und holt für jedes eine docx Datei*/
            List<LetterObj> letterList = new List<LetterObj>();
            // TODO
            foreach (var letter in library.Letters.Where(x => library.Metas[x.Key].Sort.Year >= years.Item1 && library.Metas[x.Key].Sort.Year <= years.Item2))
            {
                var letterObj = CreateLetterObj(letter, library);
                letterList.Add(letterObj);
            }
            return letterList;
        }

        public static List<LetterObj> Lib2List(ILibrary library, int year)
        {
            /*Da die Erstellung von Volume.docx dateien über das Erstellen und mergen von Einzelbriefdokumenten funtkioniert, 
             * ist diese Überladung von Lib2List nötig.
            Erstellt eine Liste von LetterObj. des Entpsrechenden Bandes und holt für jedes eine Temporäre docx Datei*/

            List<LetterObj> letterList = new List<LetterObj>();
            foreach (var letter in library.Letters.Where(x => library.Metas[x.Key].ZH != null && library.Metas[x.Key].Sort.Year == year))
            {
                var letterObj = CreateLetterObj(letter, library);
                letterList.Add(letterObj);
            }
            
            return letterList;
        }

        //public static void MakeYearLetters(int year)
        //{
        //    Logger.Out("Dokument für Jahr " + years.Item1.ToString());
        //    string tempfolder = GetTemporaryDirectory();
        //    Logger.Out("tempfolder ist " + tempfolder);
        //    var outputPaths = new Dictionary<int, string>();
        //    var letterList = Lib2List(lib, years.Item1, tempfolder);
        //    foreach (var letter in letterList)
        //    {
        //        letter.WordDoc = CreateLetterDocx(letter.OutPutFile);
        //        StyleLetterDocx(letter);
        //        Letter2Docx(letter, true);
        //        letter.WordDoc.Save();
        //        outputPaths.Add(Int32.Parse(letter.Index), letter.OutPutFile);
        //        letter.WordDoc.Close();
        //    }
        //    outputPaths.Add(0, MakeVolumeDoc(years.Item1.ToString()));
        //    outputPaths.Add(1000000000, Editionsrichtlinien);
        //    MergeDocx(years.Item1.ToString(), outputPaths);
        //    //temporären briefdateien löschen
        //    RemoveTempFolderFiles(outputPaths, tempfolder);
        //}

        public static string MakeVolumeDoc(string year)
        {
            //erstellt das leere VolDikument für den jeweiligen hammann-Band
            //volume = GetRoman(volume);
            string volTitle = "Jahr " + year;
            string outputfile = VolumesOutputDir + "HKB_" + year + ".docx";
            YearPaths.Add(outputfile);
            string title = "Johann Georg Hamann";
            string secondTitle = "Kommentierte Briefausgabe";
            string docj = "Hrsg. von Leonard Keidel und Janina Reibold";
            string henkel = "auf Grundlage der Vorarbeiten Arthur Henkels";
            string hamannInnen1 = "unter Mitarbeit von Gregor Babelotzky, Konrad Bucher,";
            string hamannInnen2 = "Christian Großmann, Carl Friedrich Haak, Luca Klopfer,";
            string hamannInnen3 = "Johannes Knüchel, Isabel Langkabel und Simon Martens.";
            string zitier = "(Heidelberg 2020ff.)";
            string tss = "Ein Projekt der Theodor Springmann Stiftung,";
            string gs = "in Kooperation mit dem Germanistischen Seminar Heidelberg.";


            string spaceAboveMain = (10 * Int32.Parse(LineHight)).ToString();
            string doubleFontSize = (2 * Int32.Parse(SmallFontSize)).ToString();
            string doubleLineHight = (2 * Int64.Parse(MiddleLineHight)).ToString();
            string indent = ((80 * Int64.Parse(SmallFontSize)).ToString());

            var metadoc = MakeEmptyDocx(outputfile);
            CorrectMarginLastSection(metadoc);

            Run wichtig = new Run(new Text(title), new Break(), new Text(secondTitle), new Break(), new Break(), new Text(volTitle));
            BoldRun(wichtig);
            SansSerifRun(wichtig);
            wichtig.RunProperties.AppendChild(new FontSize() { Val = doubleFontSize });
            Paragraph mainHeading = metadoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(wichtig));
            ApplyParaStyle(mainHeading, "stumpf");
            mainHeading.ParagraphProperties.AppendChild(new SpacingBetweenLines() { Before = spaceAboveMain, Line = doubleLineHight, LineRule = LineSpacingRuleValues.Exact });
            mainHeading.ParagraphProperties.AppendChild(new Indentation() { Left = indent });

            Run wichtiger = new Run(new Break(), new Break(), new Break(), new Text(docj), new Break(), new Text(henkel));
            SansSerifRun(wichtiger);
            BoldRun(wichtiger);
            Paragraph hrsg = new Paragraph(new Run(new Break()), new Run(new Break()), new Run(new Break()), wichtiger);
            ApplyParaStyle(hrsg, "stumpf");
            hrsg.ParagraphProperties.AppendChild(new Indentation() { Left = indent });


            Run totalEgal = new Run(new Break(), new Break(), new Text(hamannInnen1), new Break(), new Text(hamannInnen2), new Break(), new Text(hamannInnen3), new Break(), new Text(zitier));
            SansSerifRun(totalEgal);
            Paragraph mules = new Paragraph(totalEgal);
            ApplyParaStyle(mules, "stumpf");
            mules.ParagraphProperties.AppendChild(new Indentation() { Left = indent });


            Run coops = new Run(new Text(tss), new Break(), new Text(gs));
            SansSerifRun(coops);
            Paragraph coop = new Paragraph(coops, new Run(new Break() { Type = BreakValues.Page }));
            ApplyParaStyle(coop, "stumpf");
            coop.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });
            coop.ParagraphProperties.AppendChild(new SpacingBetweenLines() { Before = spaceAboveMain });


            Hyperlink haLink = new Hyperlink(new Run(new Text("www.hamann-ausgabe.de"))) { Anchor = "Hamann-Ausgabe online", DocLocation = "https://www.hamann-ausgabe.de" };
            Paragraph linkPara = new Paragraph(new Run(new Text("Stand:" + DateTime.Now.ToString(" d/M/yyyy")) { Space = SpaceProcessingModeValues.Preserve }, new TabChar()), haLink, new Break() { Type = BreakValues.Page });
            ApplyParaStyle(linkPara, "fußzeile");
            linkPara.ParagraphProperties.AppendChild(new Tabs(new TabStop { Val = TabStopValues.Right, Position = 7807 }));
            linkPara.ParagraphProperties.AppendChild(new Indentation() { Left = indent });
            linkPara.ParagraphProperties.AppendChild(new SpacingBetweenLines() { Before = spaceAboveMain, LineRule = LineSpacingRuleValues.Exact });
            linkPara.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });


            mainHeading.InsertAfterSelf(hrsg);
            hrsg.InsertAfterSelf(mules);
            mules.InsertAfterSelf(linkPara);
            linkPara.InsertAfterSelf(coop);
            coop.InsertBeforeSelf(new Paragraph(new Run()));
            var last = coop.InsertAfterSelf(new Paragraph(new Run()));
            ApplyParaStyle(last, "stumpf");
            var lastsection = last.ParagraphProperties.AppendChild(new SectionProperties());
            lastsection.AppendChild(new SectionType() { Val = SectionMarkValues.Continuous });
            lastsection.AppendChild(new Columns());

            metadoc.Save();
            metadoc.Close();
            return outputfile;
        }

        public static WordprocessingDocument MakeCommentDoc(string comment)
        {
            //erstellt ein leeres Dokument für den jeweiligen kommentar. Kommentarname und titel sind identisch, was, wenn sich was ändert, zu problemen führen könnte

            string volTitle = comment;
            string outputfile = VolumesOutputDir + "HKB_" + comment + ".docx";
            RegisterPaths.Add(outputfile);
            Console.WriteLine(outputfile);
            string title = "Johann Georg Hamann";
            string secondTitle = "Kommentierte Briefausgabe";
            string docj = "Hrsg. von Leonard Keidel und Janina Reibold";
            string henkel = "auf Grundlage der Vorarbeiten Arthur Henkels";
            string hamannInnen1 = "unter Mitarbeit von Gregor Babelotzky, Konrad Bucher,";
            string hamannInnen2 = "Christian Großmann, Carl Friedrich Haak, Luca Klopfer,";
            string hamannInnen3 = "Johannes Knüchel, Isabel Langkabel und Simon Martens.";
            string zitier = "(Heidelberg 2020ff.)";
            string tss = "Ein Projekt der Theodor Springmann Stiftung,";
            string gs = "in Kooperation mit dem Germanistischen Seminar Heidelberg.";


            string spaceAboveMain = (10 * Int32.Parse(LineHight)).ToString();
            string doubleFontSize = (2 * Int32.Parse(SmallFontSize)).ToString();
            string doubleLineHight = (2 * Int64.Parse(MiddleLineHight)).ToString();
            string indent = ((80 * Int64.Parse(SmallFontSize)).ToString());

            var metadoc = MakeEmptyDocx(outputfile);

            Run wichtig = new Run(new Text(title), new Break(), new Text(secondTitle), new Break(), new Break(), new Text(volTitle));
            BoldRun(wichtig);
            SansSerifRun(wichtig);
            wichtig.RunProperties.AppendChild(new FontSize() { Val = doubleFontSize });
            Paragraph mainHeading = metadoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(wichtig));
            ApplyParaStyle(mainHeading, "stumpf");
            mainHeading.ParagraphProperties.AppendChild(new SpacingBetweenLines() { Before = spaceAboveMain, Line = doubleLineHight, LineRule = LineSpacingRuleValues.Exact });
            mainHeading.ParagraphProperties.AppendChild(new Indentation() { Left = indent });

            Run wichtiger = new Run(new Break(), new Break(), new Break(), new Text(docj), new Break(), new Text(henkel));
            SansSerifRun(wichtiger);
            BoldRun(wichtiger);
            Paragraph hrsg = new Paragraph(new Run(new Break()), new Run(new Break()), new Run(new Break()), wichtiger);
            ApplyParaStyle(hrsg, "stumpf");
            hrsg.ParagraphProperties.AppendChild(new Indentation() { Left = indent });


            Run totalEgal = new Run(new Break(), new Break(), new Text(hamannInnen1), new Break(), new Text(hamannInnen2), new Break(), new Text(hamannInnen3), new Break(), new Text(zitier));
            SansSerifRun(totalEgal);
            Paragraph mules = new Paragraph(totalEgal);
            ApplyParaStyle(mules, "stumpf");
            mules.ParagraphProperties.AppendChild(new Indentation() { Left = indent });

            Run haLink = new Run(new Text("www.hamann-ausgabe.de"));
            Paragraph linkPara = new Paragraph(new Run(new Text("Stand:" + DateTime.Now.ToString(" d/M/yyyy")) { Space = SpaceProcessingModeValues.Preserve }, new TabChar()), haLink, new Break() { Type = BreakValues.Page });
            ApplyParaStyle(linkPara, "fußzeile");
            linkPara.ParagraphProperties.AppendChild(new Tabs(new TabStop { Val = TabStopValues.Right, Position = 7807 }));
            linkPara.ParagraphProperties.AppendChild(new Indentation() { Left = indent });
            linkPara.ParagraphProperties.AppendChild(new SpacingBetweenLines() { Before = spaceAboveMain, LineRule = LineSpacingRuleValues.Exact });
            linkPara.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });

            Run coops = new Run(new Text(tss), new Break(), new Text(gs));
            SansSerifRun(coops);
            Paragraph coop = new Paragraph(coops);
            ApplyParaStyle(coop, "stumpf");
            coop.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });
            coop.ParagraphProperties.AppendChild(new SpacingBetweenLines() { Before = spaceAboveMain });

            //um die distance nach oben von coop zu aktivieren muss man davor einen leeren absatz einfügen
            var empty2 = new Paragraph(new Run(new Text()));

            mainHeading.InsertAfterSelf(hrsg);
            hrsg.InsertAfterSelf(mules);
            mules.InsertAfterSelf(linkPara);
            /*linkPara.InsertAfterSelf(empty);
            empty.InsertAfterSelf(empty2);*/
            linkPara.InsertAfterSelf(empty2);
            empty2.InsertAfterSelf(coop);
            var last = coop.InsertAfterSelf(new Paragraph(new Run()));
            ApplyParaStyle(last, "stumpf");
            var lastsection = last.ParagraphProperties.AppendChild(new SectionProperties());
            lastsection.AppendChild(new SectionType() { Val = SectionMarkValues.NextPage });
            lastsection.AppendChild(new Columns());
            last.InsertAfterSelf(new Paragraph(new Run()));

            return metadoc;
        }


        public static void MakeRegisterComms()
        {
            //Liste der in den Briefen existierenden Referenzstellen für kommentare und erstellt die Registerkommentarbände 
            Logger.Out("Erstelle Registerbände");
            var commReferences = GetCommReferences();
            MakeNeuzeit(commReferences);
            MakeBible(commReferences);
            MakeForschung();
        }


        public static void MakeNeuzeit(Dictionary<string, Dictionary<string, List<Place>>> commReferences)
        {
            //erstellt das Register der Kommentare (früher neuzeit)
            Logger.Out("Erstelle Register-Kommentar");
            string comment = "Register";
            var commDoc = MakeCommentDoc(comment);
            StyleNeuzeitTitel(commDoc);
            var neuzeit = Letters.CommentsByCategoryLetter["neuzeit"].OrderBy(x => x.Key);
            foreach (var regChar in neuzeit)
            {
                var registerLetter = new Paragraph(new Run(new Text(regChar.Key), new Break()));
                var oldLast = GetLastPara(commDoc);
                oldLast.InsertAfterSelf(registerLetter);
                ApplyParaStyle(registerLetter, "registerKopf");
                registerLetter.ParagraphProperties.AppendChild(new KeepNext() { Val = true });

                var sortedMetaComms = regChar.OrderBy(x => x.Index);
                foreach (var metaComm in sortedMetaComms)
                {
                    MakeMetaComm(metaComm, commDoc, commReferences);
                    MakeSubComms(metaComm, commDoc, commReferences);
                    GetLastPara(commDoc).ParagraphProperties.SpacingBetweenLines.After = LineHight;
                }
                var last = GetLastPara(commDoc);
                var sectionProperties = new SectionProperties(new SectionType() { Val = SectionMarkValues.Continuous }, new Columns() { ColumnCount = 2 });
                sectionProperties.AppendChild(new PageMargin() {
                    Top = MarginTop,
                    Right = MarginLeft,
                    Bottom = MarginBottom,
                    Left = MarginLeft,
                    Footer = MarginFooter
                });
                last.ParagraphProperties.AppendChild(sectionProperties);
                last.AppendChild(new Run(new Break() { Type = BreakValues.Page }));
            }
            GetLastPara(commDoc).Descendants<Break>().Last().Remove();
            AddRegisterFooterPart(commDoc);
            CorrectMarginLastSection(commDoc, true);
            commDoc.Save();
            commDoc.Close();
            CorrectLinks(comment);
        }

        public static void AddRegisterFooterPart(WordprocessingDocument commDoc)
        {
            var main = commDoc.MainDocumentPart;
            main.DeleteParts(main.FooterParts);

            var footerpart1 = main.AddNewPart<FooterPart>("pagefooter");
            var footer1 = new Footer(GetRegisterFooterContent());
            footerpart1.Footer = footer1;

            var footerpart2 = main.AddNewPart<FooterPart>("titlefooter");
            var footer2 = new Footer(new Paragraph(new Run(new Text(" "))));
            footerpart2.Footer = footer2;

            foreach(var el in main.Document.Body.Descendants<SectionProperties>().Take(1))
            {
                el.RemoveAllChildren<FooterReference>();
                el.Append(new FooterReference() { Id = "titlefooter", Type = HeaderFooterValues.Default });
            }

            foreach (var el in main.Document.Body.Descendants<SectionProperties>().Skip(1))
            {
                el.RemoveAllChildren<FooterReference>();
                el.Append(new FooterReference() { Id = "pagefooter", Type = HeaderFooterValues.Default });
            }
        }

        public static Table GetRegisterFooterContent()
        {
            // tabellen eignen sich für das links-rechts layout, weil die anders als tabulatoren dynamische resizen
            Table table = new Table();
            TableProperties tblProp = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.None)},
                    new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.None)},
                    new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.None)},
                    new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.None)},
                    new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.None) },
                    new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.None) },
                    new TableIndentation() { Type = TableWidthUnitValues.Dxa, Width = Int32.Parse(LineHight)}
                )
            );
            table.AppendChild<TableProperties>(tblProp);

            TableRow tr = new TableRow();
            
            TableCell tc1 = new TableCell();
            var links = new Paragraph();
            ApplyParaStyle(links, "fußnote");
            tc1.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "50"}));
            var linksRun = new Run(new Text("www.hamann-ausgabe.de" + DateTime.Now.ToString(" (d/M/yyyy)")));
            links.AppendChild(linksRun);
            tc1.Append(links);
           
            TableCell tc2 = new TableCell();
            tc2.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "50" }));
            var rechtsRun = new Run(new SimpleField() {Instruction = "PAGE"});
            var rechts = new Paragraph(rechtsRun);
            ApplyParaStyle(rechts, "fußnotegroß");
            rechts.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });
            tc2.Append(rechts);

            tr.Append(tc1);
            tr.Append(tc2);
            table.AppendChild(tr);
            return table;
        }

        public static void MakeMetaComm(Comment metaComm, WordprocessingDocument commDoc, Dictionary<string, Dictionary<string, List<Place>>> commReferences)
        {
            //erstellt das Titellemma für den jeweiligen eintrag
            Run lemma = new Run(new Text(XElement.Parse(metaComm.Lemma).Value));
            BoldRun(lemma);
            var lemmaPara = GetLastPara(commDoc).InsertAfterSelf(new Paragraph(lemma));
            ApplyParaStyle(lemmaPara, "einzug");
            lemmaPara.ParagraphProperties.AppendChild(new KeepNext() { Val = true });
            Paragraph lemmaDescription = lemmaPara.InsertAfterSelf(new Paragraph());
            ApplyParaStyle(lemmaDescription, "doppeleinzug");
            lemmaDescription.ParagraphProperties.AppendChild(new SpacingBetweenLines() { After = SmallDistance });
            ParseComment(XElement.Parse(metaComm.Entry), lemmaDescription);
            if (commReferences.ContainsKey(metaComm.Index))
            {
                var commReference = commReferences[metaComm.Index];
                if (commReference.ContainsKey("0"))
                {
                    var refPara = GetLastPara(commDoc).InsertAfterSelf(new Paragraph(new Run(new Text("\u261B\u202FHKB\u202F") { Space = SpaceProcessingModeValues.Preserve })));
                    var metaRef = commReference["0"];
                    PlaceReferences(metaRef, refPara);
                    ApplyParaStyle(refPara, "doppeleinzug");
                    refPara.ParagraphProperties.AppendChild(new SpacingBetweenLines() { After = SmallDistance });
                }
            }
        }

        public static void MakeSubComms(Comment metaComm, WordprocessingDocument commDoc, Dictionary<string, Dictionary<string, List<Place>>> commReferences)
        {
            //erstellt die dem Titellemma untergeordneten einträge, also einzelne Titel von Autoren etc.
            if (metaComm.Kommentare != null)
            {
                foreach (var comm in metaComm.Kommentare)
                {
                    if (!String.IsNullOrWhiteSpace(comm.Value.Entry)) { 
                        var subLemmaPara = GetLastPara(commDoc).InsertAfterSelf(new Paragraph());
                        ApplyParaStyle(subLemmaPara, "doppeleinzug");
                        ParseComment(XElement.Parse(comm.Value.Lemma), subLemmaPara);
                        foreach (var run in subLemmaPara.ChildElements.Where(x => x is Run)) { BoldRun(run as Run); };
                        var subEintragPara = subLemmaPara.InsertAfterSelf(new Paragraph());
                        ApplyParaStyle(subEintragPara, "doppeleinzug");
                        subEintragPara.ParagraphProperties.AppendChild(new SpacingBetweenLines() { After = SmallDistance });
                        ParseComment(XElement.Parse(RemoveWhiteSpaceLinebreak(comm.Value.Entry)), subEintragPara);
                        if (commReferences.ContainsKey(metaComm.Index) && commReferences[metaComm.Index].ContainsKey(comm.Key))
                        {
                            subEintragPara.ParagraphProperties.AppendChild(new SpacingBetweenLines() { After = SmallDistance });
                            var commReference = commReferences[metaComm.Index];
                            var subCommRef = commReference[comm.Key];
                            var subRefPara = GetLastPara(commDoc).InsertAfterSelf(new Paragraph(new Run(new Text("\u261B\u202FHKB\u202F") { Space = SpaceProcessingModeValues.Preserve })));
                            PlaceReferences(subCommRef, subRefPara);
                            ApplyParaStyle(subRefPara, "doppeleinzug");
                            subRefPara.ParagraphProperties.AppendChild(new SpacingBetweenLines() { After = SmallDistance });
                        }

                        if (comm.Value.Kommentare != null)
                        {
                            foreach (var c in comm.Value.Kommentare)
                            {
                                Logger.Out("\n\nEs existiert ein sub-sub-Kommentar. Darauf war ich nicht vorbereitet. \nGruß, Fritz\n" + c.Value.Lemma);
                                Warn("Es existiert ein sub - sub - Kommentar.Darauf war ich nicht vorbereitet. \nGruß, Fritz\n" + c.Value.Lemma);
                            }
                        }
                    }
                }
            }
        }

        public static void PlaceReferences(List<Place> places, Paragraph para)
        {
            
            //stattet meta- und sub-lemmata mit den referenzstellen in den briefen aus
            var orderedPlaces = places.OrderBy(x => x.Volume).ThenBy(x => x.Page).ThenBy(x => x.Line).ToList();
            foreach (Place place in orderedPlaces)
            {
                string vol = "";
                if (place.Volume != 0)
                    vol = GetRoman(place.Volume.ToString()) + "\u202F";
                //trenngeschütztes leerzeichen: \u202F, zerstört aber den automatischen umbruch
                var volume = new Run(new Text(" ") { Space = SpaceProcessingModeValues.Preserve });
                var to = new RunProperties();
                var letter = new Run(new Text(place.Letter + "\u202F(" + vol + place.PageString + "/" + place.Line.ToString() + ")") { Space = SpaceProcessingModeValues.Preserve });
                if (place != orderedPlaces.Last())
                {
                    letter.AppendChild(new Text(", ") { Space = SpaceProcessingModeValues.Preserve });
                }
                para.AppendChild(volume);
                para.AppendChild(letter);
            }
        }


        public static Paragraph BibleHeadings(IGrouping<string, Comment> testament = null)
        {
            //erstellt die titellemmata für die Bücher der Bibel
            string spaceAbove = (10 * Int32.Parse(LineHight)).ToString();
            Paragraph heading = new Paragraph();
            if (testament == null)
            {
                heading.AppendChild(new Run(new Break() { Type = BreakValues.Page }, new Text("Apokryphen"), new Break() { Type = BreakValues.Page }));
            }
            else
            {
                if (testament.Key == "N")
                {
                    heading.AppendChild(new Run(new Break() { Type = BreakValues.Page }, new Text("Neues Testament"), new Break() { Type = BreakValues.Page }));
                }
                else if (testament.Key == "A")
                {
                    heading.AppendChild(new Run(new Break() { Type = BreakValues.Page }, new Text("Altes Testament"), new Break() { Type = BreakValues.Page }));
                }
            }

            ApplyParaStyle(heading, "registerKopf");
            var paraProps = heading.ParagraphProperties;
            paraProps.AppendChild(new SpacingBetweenLines() { Before = spaceAbove, LineRule = LineSpacingRuleValues.Exact });
            paraProps.AppendChild(new Indentation() { Left = LineHight });
            paraProps.AppendChild(new ContextualSpacing() { Val = true });
            var secProps = paraProps.AppendChild(new SectionProperties());
            secProps.PrependChild<Columns>(new Columns() { ColumnCount = 1 });
            secProps.AppendChild<SectionType>(new SectionType() { Val = SectionMarkValues.Continuous });
            return heading;
        }

        public static void BibleSubLemma(KeyValuePair<string, Comment> subBook, WordprocessingDocument commDoc, Dictionary<string, List<Place>> commReference = null)
        {

            var lemmaRun = new Run(new Text(XElement.Parse(subBook.Value.Lemma).Value + ": ") { Space = SpaceProcessingModeValues.Preserve });
            IRun(lemmaRun);
            var lemma = GetLastPara(commDoc).InsertAfterSelf(new Paragraph(lemmaRun));
            if (commReference != null && commReference.ContainsKey(subBook.Key))
            {
                PlaceReferences(commReference[subBook.Key], lemma);
            }
            else
            {
                Logger.Out("Fehler! Es befinden sich keine Referenzstellen für den subcomment " + subBook.Key + " in der Hamanndatei. Dieser Fehler kann auch auftreten, wenn die Referenzen falsch angelegt werden. (z.B. wenn die subref id zwar angegeben wird, nicht aber die ref id)");
            }
            ApplyParaStyle(lemma, "bibelregister");
            /*var orderedPlaces = commReference[subBook.Key].OrderBy(p => p.Letter).ThenBy(p => p.Page).ThenBy(p => p.Line);
            foreach (var p in orderedPlaces)
            {
                var letter = new Run(new Text(p.Letter + "\u202F") { Space = SpaceProcessingModeValues.Preserve });
                var pageLine = new Run(new Text(p.Page + "/" + p.Line));
                if (p != orderedPlaces.Last())
                {
                    pageLine.AppendChild(new Text(", ") { Space = SpaceProcessingModeValues.Preserve });
                }
                SmallFont(pageLine, MiddleFontSize);
                lemma.AppendChild(letter);
                lemma.AppendChild(pageLine);
                ApplyParaStyle(lemma, "bibelregister");
            }*/
        }

        public static void BibleMetaLemma(Dictionary<string, Dictionary<string, List<Place>>> commReferences, Comment book, WordprocessingDocument commDoc)
        {
            // erstellt die Lemmata für die einzelnen Bibelverse
            var metaLemma = new Run(new Text(XElement.Parse(book.Lemma).Value));
            BoldRun(metaLemma);
            var metaLemmaPara = GetLastPara(commDoc).InsertAfterSelf(new Paragraph(metaLemma));
            ApplyParaStyle(metaLemmaPara, "stumpf");
            metaLemmaPara.ParagraphProperties.AppendChild(new KeepNext() { Val = true });
            if (commReferences.ContainsKey(book.Index))
            {
                var commReference = commReferences[book.Index];
                if (commReference.ContainsKey("0"))
                {
                    var paragraph = metaLemmaPara.InsertAfterSelf(new Paragraph());
                    var lemmaPlaces = commReference["0"];
                    PlaceReferences(lemmaPlaces, paragraph);
                }
            }
            else
            {
                Logger.Out("Der Kommentar mit der Id " + book.Index + "tauch nicht in der Liste der Kommentarreferenzen auf.");
            }
        }


        public static void MakeBible(Dictionary<string, Dictionary<string, List<Place>>> commReferences)
        {
            //erstellt den bibel-Kommentarband
            Logger.Out("Erstelle Bibelstellen-Register");
            string comment = "Bibelstellen-Register";
            var commDoc = MakeCommentDoc(comment);
            var bibel = Letters.CommentsByCategoryLetter["bibel"].OrderBy(x => x.Key);
            foreach (var testament in bibel)
            {
                //apokryphen hingen am AT; kommen jetzt in extra dict und werden in extra rubrik angefügt
                var apokryphen = new List<Comment>();

                GetLastPara(commDoc).InsertAfterSelf(BibleHeadings(testament));
                var sortetedTestament = testament.OrderBy(x => x.Order);
                foreach (var book in sortetedTestament)
                {
                    if (book.Index.Contains("apo-"))
                    {
                        apokryphen.Add(book);
                    }
                    else
                    {
                        BibleMetaLemma(commReferences, book, commDoc);
                        /*var sortedKomments = book.Kommentare.OrderBy(x => x.Value.Order);*/
                        var sortedKomments = book.Kommentare.OrderBy(x => MatchIdSubstrings(x.Key));
                        foreach (var subBook in sortedKomments)
                        {
                            if (commReferences.ContainsKey(book.Index))
                            {
                                BibleSubLemma(subBook, commDoc, commReferences[book.Index]);
                            }
                            else
                            {
                                BibleSubLemma(subBook, commDoc);
                            }
                        }
                        GetLastPara(commDoc).ParagraphProperties.AppendChild(new SpacingBetweenLines() { After = MiddleLineHight });
                    }
                }
                var sectionProps = GetLastPara(commDoc).ParagraphProperties.AppendChild(new SectionProperties());
                sectionProps.PrependChild<Columns>(new Columns() { ColumnCount = 2 });
                sectionProps.AppendChild<SectionType>(new SectionType() { Val = SectionMarkValues.Continuous });
                AddApokryphen(commDoc, apokryphen, commReferences);
            }
            foreach (var prpt in commDoc.MainDocumentPart.Document.Body.Descendants<SectionProperties>())
            {
                prpt.AppendChild(new PageMargin() { Top = MarginTop, Right = MarginRight, Bottom = MarginBottom, Left = MarginRight, Footer = MarginFooter });
            }
            AddRegisterFooterPart(commDoc);
            CorrectMarginLastSection(commDoc);
            commDoc.Save();
            commDoc.Close();
            CorrectLinks("Bibelstellen-Register");
        }


        public static void AddApokryphen(WordprocessingDocument commDoc, List<Comment> apokryphen, Dictionary<string, Dictionary<string, List<Place>>> commReferences)
        {
            if (apokryphen.Count != 0)
            {
                GetLastPara(commDoc).InsertAfterSelf(BibleHeadings());
                var sortedApo = apokryphen.OrderBy(x => x.Order);
                foreach (var book in sortedApo)
                {
                    BibleMetaLemma(commReferences, book, commDoc);
                    var sortedKomments = book.Kommentare.OrderBy(x => MatchIdSubstrings(x.Key));
                    foreach (var subBook in sortedKomments)
                    {
                        if (commReferences.ContainsKey(book.Index))
                        {
                            BibleSubLemma(subBook, commDoc, commReferences[book.Index]);
                        }
                        else
                        {
                            BibleSubLemma(subBook, commDoc);
                        }
                    }
                    GetLastPara(commDoc).ParagraphProperties.AppendChild(new SpacingBetweenLines() { After = MiddleLineHight });
                }
                var sectionProps = GetLastPara(commDoc).ParagraphProperties.AppendChild(new SectionProperties());
                sectionProps.PrependChild<Columns>(new Columns() { ColumnCount = 2 });
                sectionProps.AppendChild<SectionType>(new SectionType() { Val = SectionMarkValues.Continuous });
            }
        }

        public static Dictionary<string, Dictionary<string, List<Place>>> GetCommReferences()
        {
            /*erstellt ein verschachteltes dictionary. auf der ersten ebene werden über die 
             * id des kommentars als key die zugeordneten subrefs in dictionarys der zweiten ebene referenziert.
             auf key 0 der zweiten ebene liegen die referenzen die dem ref direkt zugeordnet sind*/
            var commReferences = new Dictionary<string, Dictionary<string, List<Place>>>();
            foreach (var marg in Letters.Marginals.Values)
            {
                // TODO: only if ZH.Any
                int volume = 0;
                if (Letters.Metas[marg.Letter].ZH != null)
                    volume = Int32.Parse(Letters.Metas[marg.Letter].ZH.Volume);
                var place = new Place(volume, marg.Letter, ConvertNumber(marg.Page), Int32.Parse(marg.Line), marg.Page);
                XElement comm = XElement.Parse(marg.Element);
                foreach (var link in comm.Elements().Where(x => x.Name.LocalName == "link"))
                {
                    string refer = link.Attribute("ref").Value;
                    string subref = "0";
                    if (link.Attribute("subref") != null)
                    {
                        subref = link.Attribute("subref").Value;
                    }
                    AddRefsToDict(refer, subref, commReferences, place);
                }
            }
            return commReferences;
        }


        public static void MakeForschung()
        {
            //erstellt die Forschungsbibliographie
            Logger.Out("Erstelle Forschungsbibliographie");
            string name = "Forschungsbibliographie";
            var commDoc = MakeCommentDoc(name);
            var forschung = Letters.CommentsByCategoryLetter["forschung"].OrderBy(x => x.Key);
            var bodySec = commDoc.MainDocumentPart.Document.Body.Descendants<SectionProperties>().First();
            
            //Seitenränder definieren
            PageMargin pageMargin = new PageMargin() { Top = MarginTop, Right = MarginRight, Bottom = MarginBottom, Left = MarginLeft, Footer = MarginFooter };
            bodySec.PrependChild(pageMargin);
            bodySec.PrependChild<Columns>(new Columns() { ColumnCount = 1 });
            bodySec.AppendChild<SectionType>(new SectionType() { Val = SectionMarkValues.Continuous });
            var cols = bodySec.GetFirstChild<Columns>();
            cols.ColumnCount = 1;

            var section = bodySec.CloneNode(true);
            foreach (var item in forschung)
            {
                var kopf = GetLastPara(commDoc).AppendChild(new Paragraph(new Run(new Text(item.Key), new Break())));
                ApplyParaStyle(kopf, "registerKopf");
                //var list = item.OfType<Comment>().ToList().OrderBy(x => x.Index);
                var list = item.OfType<Comment>().ToList().OrderBy(x => MatchForschungSubstrings(x.Index)[0]).ThenBy(x => MatchForschungSubstrings(x.Index)[1]).ThenBy(x => MatchForschungSubstrings(x.Index)[2]).ThenBy(x => MatchForschungSubstrings(x.Index)[3]);
                foreach (Comment obj in list)
                {
                    XElement lemma = XElement.Parse(obj.Lemma);
                    var entry = GetLastPara(commDoc).AppendChild(new Paragraph(new Run(new Text(lemma.Value))));
                    ApplyParaStyle(entry, "register");
                }
                var paraProps = commDoc.MainDocumentPart.Document.Body.Descendants<ParagraphProperties>().Last();
                var secProps = paraProps.AppendChild(section.CloneNode(true));
                secProps.AppendChild(new SectionType() { Val = SectionMarkValues.NextPage });
            }

            AddRegisterFooterPart(commDoc);
            CorrectMarginLastSection(commDoc);
            commDoc.Save();
            commDoc.Close();
            CorrectLinks("Forschungsbibliographie");
        }

        public static LetterObj CreateLetterObj(KeyValuePair<string, Letter> letter, ILibrary library, string tempdir = null)
        {
            //erstellt ein LetterObj aus einem Letter
            var index = letter.Value.Index;
            var meta = library.Metas[letter.Key];
            XElement letterText = StringToXElement(letter.Value.Element);
            LetterObj letterObj = new LetterObj(index, letterText, meta, letter.Key, tempdir);
            if (library.Traditions.ContainsKey(index))
            {
                var trad = library.Traditions[index].Element;
                letterObj.Tradition = StringToXElement(trad);
            }
            return letterObj;
        }


        #endregion

        #region StyleFunktionen


        public static void CreateColSection(WordprocessingDocument wordDoc, bool single = false)
        {
            //erstellt eine section innerhalb des Dokumentes und erzeugt je nach bool single 2 oder eine Spalte
            var last = GetLastPara(wordDoc);
            var props = last.ParagraphProperties;
            props.AppendChild<SectionProperties>(new SectionProperties());
            if (single)
            {
                props.SectionProperties.AppendChild(new Columns() { ColumnCount = 1, Space = ColumnDistance, EqualWidth = true });
            }
            else
            {
                props.SectionProperties.AppendChild(new Columns() { ColumnCount = 2, Space = ColumnDistance, EqualWidth = true });
            }
            props.SectionProperties.AppendChild<SectionType>(new SectionType() { Val = SectionMarkValues.Continuous });
            PageMargin pageMargin = new PageMargin() { Top = MarginTop, Right = MarginRightColumns, Bottom = MarginBottom, Left = MarginLeft, Footer = MarginFooter };
            props.SectionProperties.PrependChild(pageMargin);
        }

        public static void StyleNeuzeitTitel(WordprocessingDocument commDoc)
        {
            //erstellt den titel für den neuzeitkommentar
            GetLastPara(commDoc).AppendChild(new Run(new Break() { Type = BreakValues.Page }));
            var titelProps = commDoc.MainDocumentPart.Document.Body.Descendants<SectionProperties>().Last();
            titelProps.GetFirstChild<SectionType>().Val = SectionMarkValues.NextPage;
            titelProps.GetFirstChild<Columns>().ColumnCount = 1;
            titelProps.AppendChild(new PageMargin()
            {
                Left = MarginLeft,
                Right = MarginRight,
                Top = MarginTop,
                Bottom = MarginBottom,
                Footer = MarginFooter
            });
        }

        /*die delegates nehmen meistens nur eine run entgegen und formatieren ihn. 
         * da sich häufig mehrer formatierungen überlagern werden sie beim auswerten 
         * des xml zu multikastdelegates congglomertiert*/

        public delegate void Formatierer(Run run, string arg = null);

        public static void HighRun(Run run, string arg = null)
        {
            run.RunProperties.PrependChild<Position>(new Position() { Val = SuperValue });
        }

        public static void LowRun(Run run, string arg = null)
        {
            run.RunProperties.PrependChild<Position>(new Position() { Val = SubValue });
        }

        public static void SmallFont(Run run, string arg = null)
        {
            string größe;
            if (arg == null)
            {
                größe = SmallFontSize;
            }
            else
            {
                größe = arg;
            }
            if (run.RunProperties != null)
            {
                RunProperties runprops = run.RunProperties;
                runprops.FontSize = new FontSize() { Val = größe };
            }
            else
            {
                RunProperties runprops = run.PrependChild<RunProperties>(new RunProperties());
                runprops.FontSize = new FontSize() { Val = größe };
            }
        }

        public static void BracketAfterRun(Run run, string arg = null)
        {
            run.AppendChild(new Text("\u005D"));
        }

        public static void LinebreakSlash(Run run, string arg = null)
        {
            run.RunProperties.InsertAfterSelf<Text>(new Text("\u002F ") { Space = SpaceProcessingModeValues.Preserve });
        }

        public static void NonreadRun(Run run, string arg = null)
        {
            //run.RunProperties.InsertAfterSelf(new Text("\u00BF"));
            //run.AppendChild<Text>(new Text("\u00BF"));
            run.RunProperties.InsertAfterSelf(new SymbolChar() { Char = "000025E6", Font = SpecialFont });
            run.AppendChild(new SymbolChar() { Char = "000025E6", Font = SpecialFont });
        }

        public static void NewLineRun(Run run, string arg = null)
        {
            run.AppendChild<Break>(new Break());
        }
        public static void LineBreakBefore(Run run, string arg = null)
        {
            run.PrependChild<Break>(new Break());
        }

        public static void MarginBefore(Run run, string arg = null)
        {
            Paragraph parent = run.Parent as Paragraph;
            if (parent.ParagraphProperties == null)
            {
                parent.PrependChild<ParagraphProperties>(new ParagraphProperties());
            }
            ApplyParaStyle(parent, "ueberschrift");
        }


        public static void KeepNextRun(Run run, string arg = null) 
        {
            Paragraph parent = run.Parent as Paragraph;
            if (parent.ParagraphProperties == null)
            {
                parent.PrependChild<ParagraphProperties>(new ParagraphProperties());
            }
            parent.ParagraphProperties.AppendChild(new KeepNext() { Val = true });
        }

        public static void GreyBackRun(Run run, string arg = null)
        {
            RunProperties runprops = run.RunProperties;
            runprops.AppendChild(new Shading() { Fill = "#e2e2e2" });
        }

        public static void SalutationRun(Run run, string arg = null)
        {

            Paragraph parent = run.Parent as Paragraph;
            if (parent.ParagraphProperties != null && parent.ParagraphProperties.ParagraphStyleId != null && parent.ParagraphProperties.ParagraphStyleId.Val == "stumpf")
            {
                parent.ParagraphProperties.ParagraphStyleId.Val = "anrede";
            }
            else
            {
                if (parent.ParagraphProperties == null)
                {
                    parent.PrependChild<ParagraphProperties>(new ParagraphProperties());
                }
                if (parent.ParagraphProperties.ParagraphStyleId == null)
                {
                    parent.ParagraphProperties.AppendChild<ParagraphStyleId>(new ParagraphStyleId() { Val = "anrede" });
                }
                if (parent.ParagraphProperties.ParagraphStyleId.Val != "stumpf" && parent.ParagraphProperties.ParagraphStyleId.Val != "anrede")
                {
                    parent.ParagraphProperties.AppendChild<SpacingBetweenLines>(new SpacingBetweenLines() { After = LineHight, Line = LineHight, LineRule = LineSpacingRuleValues.Exact });
                }
            }
        }

        public static void SansSerifRun(Run run, string arg = null)
        {
            var test = run.Parent; 
            RunProperties runprops = run.RunProperties;
            if (runprops == null)
            {
                runprops = run.PrependChild(new RunProperties());
            }
            runprops.AppendChild(new RunFonts() { Ascii = Hamann2Word.SpecialFont, HighAnsi = Hamann2Word.SpecialFont, ComplexScript = Hamann2Word.SpecialFont });
        }

        public static void DiodoneRun(Run run, string arg = null)
        {
            RunProperties runprops = run.RunProperties;
            if (runprops == null)
            {
                runprops = run.PrependChild(new RunProperties());
            }
            runprops.AppendChild(new RunFonts() { Ascii = Hamann2Word.Diodone, HighAnsi = Hamann2Word.Diodone, ComplexScript = Hamann2Word.Diodone });
            runprops.AppendChild(new FontSize() { Val = "21" });
            runprops.AppendChild(new FontSizeComplexScript() { Val ="21" });
        }

        public static void SerifRun(Run run, string arg = null)
        {
            RunProperties runprops = run.RunProperties;
            runprops.AppendChild(new RunFonts() { Ascii = Hamann2Word.NormalFont, HighAnsi = Hamann2Word.NormalFont, ComplexScript = Hamann2Word.NormalFont });
        }

        public static void BoldRun(Run run, string arg = null)
        {
            RunProperties runprops = run.RunProperties;
            if (runprops == null)
            {
                runprops = run.PrependChild<RunProperties>(new RunProperties());
            }
            runprops.AppendChild(new Bold() { Val = OnOffValue.FromBoolean(true) });
        }

        public static void IRun(Run run, string arg = null)
        {
            RunProperties runprops = run.RunProperties;
            if (runprops == null)
            {
                runprops = run.PrependChild<RunProperties>(new RunProperties());
            }
            runprops.AppendChild(new Italic() { Val = OnOffValue.FromBoolean(true) });
            //return run;
        }

        public static void GreyRun(Run run, string arg = null)
        {
            RunProperties runprops = run.RunProperties;
            if (runprops == null)
            {
                runprops = run.AppendChild(new RunProperties());
            }
            runprops.AppendChild(new Color() { Val = "#7d7d74" });
        }

        public static void HyperLink(Run run, string arg)
        {
            /*macht man KEINE Links: externe Links müssen über die main section des Dokuments referenziert werden.
             * das führt aber zu problemen mit parallelen zugriffen auf die Datei. Also mache ich das zuerst falsch (hier)
             * und korriegiere zum schluss alle links im dokument*/
            new Hyperlink(run) { Id = arg };
        }

        public static void UlRun(Run run, string arg = null)
        {
            RunProperties runprops = run.RunProperties;
            runprops.AppendChild(new Underline() { Val = UnderlineValues.Single });
        }

        public static void DulRun(Run run, string arg = null)
        {
            RunProperties runprops = run.RunProperties;
            runprops.AppendChild(new Underline() { Val = UnderlineValues.Double });
            //return run;
        }

        public static void TrulRun(Run run, string arg = null)
        {
            RunProperties runprops = run.RunProperties;
            runprops.AppendChild(new Underline() { Val = UnderlineValues.Thick });
            //return run;
        }

        public static void StrikeRun(Run run, string arg = null)
        {
            /*durchgestrichene Runs formatieren; dafür muss man 
             * testen ob sich die durchstreichungen verschachteln*/

            RunProperties runprops = run.RunProperties;
            if (runprops.Strike != null)
            {
                if (runprops.Strike.Val)
                {
                    runprops.Strike.Val = false;
                    runprops.AppendChild(new DoubleStrike() { Val = true });
                }
            }
            else
            {
                runprops.AppendChild(new Strike() { Val = true });
            }
        }

        public static void FrameCounterParagraph(Paragraph para)
        {
            para.ParagraphProperties.PrependChild<KeepNext>(new KeepNext() { Val = true });
            para.ParagraphProperties.PrependChild<FrameProperties>(new FrameProperties
            {
                Width = "1000", //ist nur ein dummy wert, damit die box breitgenug ist, denn ich muss die width fix setzten, sonst fließt der umgebende text immer falsch (trotz wrap ...)
                Wrap = TextWrappingValues.Around,
                HorizontalPosition = HorizontalAnchorValues.Page,
                VerticalPosition = VerticalAnchorValues.Text,
                XAlign = HorizontalAlignmentValues.Left,
                Y = "0",
                HorizontalSpace = FooterToText
            });

        }


        public static void MakeFramedEmptyLines(WordprocessingDocument wordDoc)
        {
            //die leeren Zeilen mit textrahmen dienen als abstandhalter bei überschriften
            var lastParagraph = GetLastPara(wordDoc);
            try
            {
                while (lastParagraph.ParagraphProperties.SectionProperties == null)
                {
                    lastParagraph = lastParagraph.PreviousSibling<Paragraph>();
                }
            }
            catch (NullReferenceException)
            {
                lastParagraph = GetLastPara(wordDoc);
            }

            //lastParagraph = lastParagraph.PreviousSibling<Paragraph>();
            var empty1 = new Paragraph(new Run());
            var empty2 = new Paragraph(new Run());
            ApplyParaStyle(empty1, "stumpf");
            ApplyParaStyle(empty2, "stumpf");
            FrameHeadingParagraph(empty1);
            FrameHeadingParagraph(empty2);
            lastParagraph = lastParagraph.InsertAfterSelf(empty1);
            lastParagraph.InsertAfterSelf(empty2);
        }

        public static void FrameHeadingParagraph(Paragraph para)
        {
            //die headings müssen in einen frame, weil sie sonst in einer der spalten dargestellt werden, die sie betiteln sollen
            para.ParagraphProperties.PrependChild<FrameProperties>(new FrameProperties
            {
                Width = "8000",
                Wrap = TextWrappingValues.Around,
                HorizontalPosition = HorizontalAnchorValues.Text,
                VerticalPosition = VerticalAnchorValues.Text,
                HorizontalSpace = FooterToText
            });
        }

        public static void PosRight(WordprocessingDocument wordDoc)
        {
            /*formatiert den letzten absatz eines worddokuments rechtsbündig
             * da rechtsbündige textteile ja auch in zeilen vorkommen in denen linksbündiger text
             * existiert, müssen die teile teilweise in eine textboxt*/

            Paragraph lastpara = GetLastPara(wordDoc);

            // TODO WAS LÄUFT HIER FALSCH
            if (lastpara != null && lastpara.ParagraphProperties.ParagraphStyleId != null)
            {
                string lastStyle = lastpara.ParagraphProperties.ParagraphStyleId.Val;
                if (lastStyle != "rechtsbündig" && lastStyle != "einzug" && lastStyle != "doppeleinzug" && lastStyle != "dreifacheinzug" && lastStyle != "vierfacheinzug" && lastStyle != "fünffacheinzug" && lastStyle != "sechsfacheinzug" && lastStyle != "siebenfacheinzug")
                {
                    if (CheckForContent(lastpara))
                    {
                        Paragraph posRightPara = lastpara.InsertAfterSelf<Paragraph>(new Paragraph());
                        ApplyParaStyle(posRightPara, "rechtsbündig");
                        FramePos(posRightPara, "right");
                    }
                    else
                    {
                        ApplyParaStyle(lastpara, "rechtsbündig");
                    }
                }
                else
                {
                    Paragraph posRightPara = lastpara.InsertAfterSelf<Paragraph>(new Paragraph());
                    ApplyParaStyle(posRightPara, "rechtsbündig");
                    FramePos(posRightPara, "right");
                }
            }
        }

        public static void PosCenter(WordprocessingDocument wordDoc)
        {
            /*zentriert einen absatz. wenn der letzte absatz leer ist, wird er entpsrechend formatiert
             wenn nicht, wird ein neuer erzeugt, wenn der letzte absatz schon zentriert ist, passiert nix*/
            Paragraph lastpara = GetLastPara(wordDoc);
            if (lastpara != null)
            {
                if (lastpara.ParagraphProperties.ParagraphStyleId.Val != "zentriert")
                {
                    if (CheckForContent(lastpara))
                    {
                        Paragraph posCenterPara = lastpara.InsertAfterSelf<Paragraph>(new Paragraph());
                        ApplyParaStyle(posCenterPara, "zentriert");
                        FramePos(posCenterPara, "center");
                    }
                    else
                    {
                        ApplyParaStyle(lastpara, "zentriert");
                    }
                }
            }
            else
            {
                Logger.Out("Erster Absatz des gesamten Dokuments ist zentriert, das sollte eigentlich nicht vorkommen.");
                var newFirstPara = wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph());
                ApplyParaStyle(newFirstPara, "zentriert");
            }
        }

        public static void FramePos(Paragraph para, string pos = "")
        {
            //setzt einen paragraphen in einen textramen und positioniert ihn entsprechend des pos atributes einer xml align zeile
            if (pos == "right")
            {
                string yVal = "-" + LineHight;
                if (para.PreviousSibling<Paragraph>().ParagraphProperties.ParagraphStyleId.Val == "anrede")
                {
                    yVal = "-" + ColumnDistance;
                }
                para.ParagraphProperties.PrependChild<FrameProperties>(new FrameProperties
                {
                    Wrap = TextWrappingValues.None,
                    HorizontalPosition = HorizontalAnchorValues.Margin,
                    VerticalPosition = VerticalAnchorValues.Text,
                    XAlign = HorizontalAlignmentValues.Right,
                    Y = yVal,
                    HorizontalSpace = FooterToText
                }); ;
            }

            else if (pos == "center")
            {
                para.PreviousSibling<Paragraph>().ParagraphProperties.PrependChild<KeepNext>(new KeepNext() { Val = true });
                para.ParagraphProperties.PrependChild<FrameProperties>(new FrameProperties
                {
                    Wrap = TextWrappingValues.Through,
                    HorizontalPosition = HorizontalAnchorValues.Text,
                    VerticalPosition = VerticalAnchorValues.Text,
                    XAlign = HorizontalAlignmentValues.Center,
                    Y = "-" + LineHight,
                    HorizontalSpace = FooterToText
                });
            }
        }

        public static SectionProperties AppendFooter(MainDocumentPart main, Run run)
        {
            //fügt dem Dokument einen footer an. run ist ein dokumentspezifischer string z.B. band etc
            SectionProperties secProps = new SectionProperties();
            FooterPart footerPart = main.AddNewPart<FooterPart>("default");
            var link = new Run(new Text("www.hamann-ausgabe.de"));
            Paragraph foo = new Paragraph(link, new Run(new Text(DateTime.Now.ToString(" (d/M/yyyy)")) { Space = SpaceProcessingModeValues.Preserve }, new TabChar()), run);
            ApplyParaStyle(foo, "fußzeile");
            foo.ParagraphProperties.AppendChild(new Tabs(new TabStop { Val = TabStopValues.Right, Position = 7807 }));
            Footer footer = new Footer(foo);
            footerPart.Footer = footer;
            FooterReference footerReference = new FooterReference() { Type = DocumentFormat.OpenXml.Wordprocessing.HeaderFooterValues.Default, Id = "default" };
            secProps.InsertAt(footerReference, 0);
            return secProps;
        }

        static public void ApplyParaStyle(Paragraph para, string id)
        {
            if (para.Elements<ParagraphProperties>().Count() == 0)
            {
                para.PrependChild<ParagraphProperties>(new ParagraphProperties());
            }

            ParagraphProperties paraProps = para.ParagraphProperties;

            if (paraProps.ParagraphStyleId == null)
            {
                paraProps.ParagraphStyleId = new ParagraphStyleId();
            }

            paraProps.ParagraphStyleId.Val = id;
        }

        public static Run CreateFooterRun(LetterObj letter)
        {
            //erzeugt bandspezifischen run für footer
            String zh = MakeZHString(letter, false);
            Run footerRun = new Run(new Text("HKB\u202F" + letter.Autopsic + " " + "(" + zh + ")") { Space = SpaceProcessingModeValues.Preserve });
            return footerRun;
        }

        static public void CreateStyles(WordprocessingDocument wordDoc)
        {
            //Absatzvorlagen definitionen 
            string standardStyleName = "stumpf";
            string indentStyle = "einzug";
            string indentValue = LineHight;
            string doubleIndentValue = ((Int32.Parse(indentValue)) * 2).ToString();
            string tripleIndentValue = ((Int32.Parse(indentValue)) * 3).ToString();
            string quadrupleIndentValue = ((Int32.Parse(indentValue)) * 4).ToString();
            string fünfIndentValue = ((Int32.Parse(indentValue)) * 5).ToString();
            string sechsIndentValue = ((Int32.Parse(indentValue)) * 6).ToString();
            string siebenIndentValue = ((Int32.Parse(indentValue)) * 7).ToString();

            Style stumpf = CreateParaStyle(wordDoc, standardStyleName, standardStyleName);
            stumpf.Default = true;

            CreateParaStyle(wordDoc, indentStyle, indentStyle, standardStyleName, standardStyleName, indentValue);

            CreateParaStyle(wordDoc, "doppeleinzug", "doppeleinzug", standardStyleName, standardStyleName, doubleIndentValue);

            CreateParaStyle(wordDoc, "dreifacheinzug", "dreifacheinzug", standardStyleName, standardStyleName, tripleIndentValue);

            CreateParaStyle(wordDoc, "vierfacheinzug", "vierfacheinzug", standardStyleName, standardStyleName, quadrupleIndentValue);

            CreateParaStyle(wordDoc, "fünffacheinzug", "fünffacheinzug", standardStyleName, standardStyleName, fünfIndentValue);

            CreateParaStyle(wordDoc, "sechsfacheinzug", "sechsfacheinzug", standardStyleName, standardStyleName, sechsIndentValue);

            CreateParaStyle(wordDoc, "siebenfacheinzug", "siebenfacheinzug", standardStyleName, standardStyleName, siebenIndentValue);

            var tradPara = CreateParaStyle(wordDoc, "überlieferung", "überlieferung", standardStyleName, standardStyleName);
            var runP = tradPara.ChildElements.First<StyleRunProperties>();
            runP.AppendChild(new RunFonts() { Ascii = SpecialFont, HighAnsi = SpecialFont, ComplexScript = SpecialFont });

            Style fnStyle = CreateParaStyle(wordDoc, "fußnote", "fußnote", standardStyleName, standardStyleName);
            ParagraphProperties fnStyleProps = fnStyle.ChildElements.First<ParagraphProperties>();
            fnStyleProps.AppendChild(new SpacingBetweenLines() { After = "0", Before = "0", Line = MiddleLineHight, LineRule = LineSpacingRuleValues.Exact });
            var fnRunPr = fnStyle.ChildElements.First<StyleRunProperties>();
            fnRunPr.FontSize.Val = MiddleFontSize;

            Style fnGStyle = CreateParaStyle(wordDoc, "fußnotegroß", "fußnotegroß", standardStyleName, standardStyleName);
            ParagraphProperties fnGStyleProps = fnGStyle.ChildElements.First<ParagraphProperties>();
            fnGStyleProps.AppendChild(new SpacingBetweenLines() { After = "0", Before = "0", Line = MiddleLineHight, LineRule = LineSpacingRuleValues.Exact });
            var fnGRunPr = fnGStyle.ChildElements.First<StyleRunProperties>();
            fnGRunPr.FontSize.Val = FontSize;
            fnGRunPr.AppendChild(new Bold());

            CreateParaStyle(wordDoc, "zeilenzählung", "zeilenzählung", standardStyleName, standardStyleName, justification: "right");

            CreateParaStyle(wordDoc, "seitenzählung", "seitenzählung", standardStyleName, standardStyleName, justification: "right");

            CreateParaStyle(wordDoc, "zentriert", "zentriert", standardStyleName, standardStyleName, "0", "center");

            CreateParaStyle(wordDoc, "rechtsbündig", "rechtsbündig", standardStyleName, standardStyleName, justification: "right");

            Style edition = CreateParaStyle(wordDoc, "textkritik", "textkritik", "textkritik", standardStyleName, justification: "left");
            ParagraphProperties editionProperties = edition.ChildElements.First<ParagraphProperties>();
            editionProperties.AppendChild<Indentation>(new Indentation() { Left = indentValue, Hanging = indentValue });

            Style comm = CreateParaStyle(wordDoc, "kommentar", "kommentar", "kommentar", standardStyleName, justification: "left");
            ParagraphProperties commProperties = comm.ChildElements.First<ParagraphProperties>();
            // BUG commProperties.AppendChild(new KeepNext() {Val = true });
            commProperties.AppendChild<Indentation>(new Indentation() { Left = indentValue, Hanging = indentValue });
            StyleRunProperties commRunProperties = comm.ChildElements.First<StyleRunProperties>();
            commRunProperties.AppendChild(new RunFonts() { Ascii = SpecialFont, HighAnsi = SpecialFont, ComplexScript = SpecialFont });

            Style salutation = CreateParaStyle(wordDoc, "anrede", "anrede", standardStyleName, standardStyleName, justification: "left");
            salutation.ChildElements.First<ParagraphProperties>().SpacingBetweenLines.After = LineHight;

            Style ueberschrift = CreateParaStyle(wordDoc, "ueberschrift", "ueberschrift", standardStyleName, standardStyleName, justification: "left");
            var LineHeight = Int32.Parse(LineHight) / 2;
            ueberschrift.ChildElements.First<ParagraphProperties>().SpacingBetweenLines.Before = LineHight;
            ueberschrift.ChildElements.First<ParagraphProperties>().SpacingBetweenLines.After = LineHeight.ToString();

            Style footerStyle = CreateParaStyle(wordDoc, "fußzeile", "fußzeile", "fußzeile", "linksbündig", justification: "left");
            ParagraphProperties paraProps = footerStyle.ChildElements.First<ParagraphProperties>();
            paraProps.SpacingBetweenLines.Before.Value = FooterToText;
            paraProps.SpacingBetweenLines.Line = MiddleLineHight;
            paraProps.SpacingBetweenLines.LineRule = LineSpacingRuleValues.Exact;
            StyleRunProperties footerRunProps = footerStyle.ChildElements.First<StyleRunProperties>();
            footerRunProps.AppendChild(new Color() { Val = "#7d7d74" });
            footerRunProps.AppendChild(new FontSize() { Val = MiddleFontSize });
            footerRunProps.AppendChild(new RunFonts() { Ascii = SpecialFont, HighAnsi = SpecialFont, ComplexScript = SpecialFont });

            Style sourceSection = CreateParaStyle(wordDoc, "quelle", "quelle", "quelle", standardStyleName, justification: "left");
            ParagraphProperties sourceProps = sourceSection.ChildElements.First<ParagraphProperties>();
            sourceProps.SpacingBetweenLines.Before.Value = FooterToText;
            sourceProps.SpacingBetweenLines.Line = MiddleLineHight;
            sourceProps.SpacingBetweenLines.LineRule = LineSpacingRuleValues.Exact;

            StyleRunProperties sourceRunProps = sourceSection.AppendChild(new StyleRunProperties());
            var sourceFontSize = new FontSize() { Val = MiddleFontSize };
            var sourceFonts = new RunFonts() { Ascii = SpecialFont, HighAnsi = SpecialFont, ComplexScript = SpecialFont };
            sourceRunProps.Append(sourceFontSize);
            sourceRunProps.Append(sourceFonts);

            Style register = CreateParaStyle(wordDoc, "register", "register", "register", standardStyleName);
            var regProps = register.ChildElements.First<ParagraphProperties>();
            regProps.Indentation.Left = ColumnDistance;
            regProps.Indentation.Hanging = indentValue;
            regProps.SpacingBetweenLines.After = (Int32.Parse(LineHight) / 2).ToString();
            regProps.AppendChild(new ContextualSpacing() { Val = false });

            Style registerChar = CreateParaStyle(wordDoc, "registerKopf", "registerKopf", "registerKopf", standardStyleName);
            var charParaProps = registerChar.ChildElements.First<ParagraphProperties>();
            charParaProps.AppendChild(new KeepNext() { Val = true });
            charParaProps.AppendChild(new ContextualSpacing() { Val = true });
            var charProps = registerChar.ChildElements.First<StyleRunProperties>();
            charProps.AppendChild(new Bold() { Val = OnOffValue.FromBoolean(true) });
            charProps.AppendChild(new FontSize() { Val = BigFontSize });

            Style bibelregister = CreateParaStyle(wordDoc, "bibelregister", "bibelregister", "bibelregister", standardStyleName);
            var bibRegProps = bibelregister.ChildElements.First<ParagraphProperties>();
            bibRegProps.Indentation.Left = ColumnDistance;
            bibRegProps.Indentation.Hanging = indentValue;
        }

        public static Paragraph MakeLetterNrPara(LetterObj letter)
        {
            string nr = letter.Autopsic;
            Run nrRun = new Run(new Text(nr));
            nrRun.PrependChild(new RunProperties(new FontSize() { Val = "32" }));
            Paragraph para = new Paragraph();
            para.AppendChild(nrRun);
            ApplyParaStyle(para, "linksbündig");
            BoldRun(nrRun);
            return para;
        }

        public static Paragraph MakeZhPara(LetterObj letter)
        {
            Paragraph para = new Paragraph();
            //string zh = MakeZHString(letter);
            //Run zhRun = new Run(new Text(zh));
            //Run zhRun = CreateFooterRun(letter);
            var zhRun = new Run(new Text(MakeZHString(letter, true)));
            SmallFont(zhRun);
            BoldRun(zhRun);
            ApplyParaStyle(para, "seitenzählung");
            FrameCounterParagraph(para);
            para.AppendChild(zhRun);
            para.AppendChild(new Run(new Break()));
            return para;
        }

        public static Paragraph MakeMetaPara(LetterObj letter)
        {
            //erzeugt einen absatz mit dem sender und empfänger etc. des briefes
            Paragraph para = new Paragraph();
            ApplyParaStyle(para, "linksbündig");
            string date = letter.Meta.Date;
            Run dateRun = new Run(new Text(date));
            BoldRun(dateRun);
            para.AppendChild<Run>(dateRun);
            para.AppendChild<Run>(new Run(new Break()));
            List<string> senderIndexList = letter.Meta.Senders;
            List<string> recieverIndexList = letter.Meta.Receivers;
            List<string> senderList = new List<string>();
            List<string> recieverList = new List<string>();

            if (senderIndexList.Count != 0)
            {
                foreach (string send in senderIndexList)
                {
                    senderList.Add(
                        GetPerson(send));
                }
            }
            if (recieverIndexList.Count != 0)
            {
                foreach (string recieve in recieverIndexList)
                {
                    recieverList.Add(GetPerson(recieve));
                }
            }
            string delimiter = ", ";
            Run traffic = new Run();
            BoldRun(traffic);
            if (senderList.Count != 0 && recieverList.Count != 0)
            {
                string senders = senderList.Aggregate((i, j) => i + delimiter + j);
                string recievers = recieverList.Aggregate((i, j) => i + delimiter + j);
                traffic.AppendChild<Text>(new Text(senders + " \u2192 " + recievers));
            }
            else if (senderList.Count != 0)
            {
                string senders = senderList.Aggregate((i, j) => GetPerson(i) + delimiter + GetPerson(j));
                traffic.AppendChild<Text>(new Text(senders + " \u2192 " + "unbekannt"));
            }
            else if (recieverList.Count != 0)
            {
                string recievers = recieverList.Aggregate((i, j) => GetPerson(i) + delimiter + GetPerson(j));
                traffic.AppendChild<Text>(new Text("unbekannt" + " \u2192 " + recievers));

            }

            para.AppendChild<Run>(traffic);
            para.AppendChild<Run>(new Run(new Break()));
            return para;
        }

        public static void ParseXElement(XElement list, Paragraph para, LetterObj letter) {
            //loopt über die Nodes des BriefXmls 
            foreach (XNode xnode in list.Nodes())
            {
                /*wenn die node der ersten ebene ein XElelemnt ist, werden die 
                * ihrem typ entsprechenden formatierungs informationen auf den "StyleStack" gelegt.
                dann wird mit WalkNodeTree über die ChildNodes geloopt*/
                if (xnode is XElement)
                {
                    XElement xelem = xnode as XElement;
                    Formatierer stack = null;
                    stack = ProcessXelement(stack, xelem, letter.WordDoc, letter);
                    if (xelem.LastNode != null)
                    {
                        WalkNodeTree(xelem.LastNode, stack, para, letter.WordDoc, letter);
                    }
                }
                /*wenn node der ersten ebene XText ist, wird ihr Textinhalt als Run dem letzten Absatz des Dokuments angehängt*/
                else if (xnode is XText)
                {
                    Paragraph lastParagraph = GetLastPara(letter.WordDoc);
                    MakeRun(lastParagraph, xnode);
                }
            }  
        }

        public static void ParseTraditionXElement(XElement list, Paragraph para, LetterObj letter) {
            //loopt über die Nodes des BriefXmls 
            foreach (XNode xnode in list.Nodes())
            {
                /*wenn die node der ersten ebene ein XElelemnt ist, werden die 
                * ihrem typ entsprechenden formatierungs informationen auf den "StyleStack" gelegt.
                dann wird mit WalkNodeTree über die ChildNodes geloopt*/
                if (xnode is XElement)
                {
                    XElement xelem = xnode as XElement;
                    Formatierer stack = null;
                    stack = ProcessXelement(stack, xelem, letter.WordDoc, letter);
                    if (xelem.LastNode != null)
                    {
                        WalkNodeTree(xelem.LastNode, stack, para, letter.WordDoc, letter);
                    }
                }
                /*wenn node der ersten ebene XText ist, wird ihr Textinhalt als Run dem letzten Absatz des Dokuments angehängt*/
                else if (xnode is XText)
                {
                    Run run = MakeTextRun(xnode);
                    Formatierer stack = null;
                    foreach (var anc in xnode.Ancestors())
                    {
                        stack += GetFormat(anc, para: para);
                    }
                    stack?.Invoke(run);
                    Paragraph lastParagraph = GetLastPara(letter.WordDoc);
                    lastParagraph.AppendChild(run);
                }
            }  
        }

        public static void ParseSublementaXElement(XElement list, Paragraph para, LetterObj letter = null)
        {
            //wertet Kommentare, Überliefrerungen und Varianten apparat in form von XElement aus und hängt sie an para an 
            if (list != null)
            {
                var singleNodes = list.DescendantNodes();
                foreach (var node in singleNodes)
                {   
                    if (node is XElement && (node as XElement).IsEmpty)
                    {
                        Run run = MakeTextRun();
                        Formatierer stack = GetFormat(node, run, para);
                        stack?.Invoke(run);
                        para.AppendChild(run);
                    }
                    else if (node is XText)
                    {
                        Run run = MakeTextRun(node);
                        Formatierer stack = null;
                        foreach (var anc in node.Ancestors())
                        {
                            stack += GetFormat(anc, para: para);
                        }
                        stack?.Invoke(run);
                        para.AppendChild(run);
                    }
                }
            }
            else
            {
                Logger.Out("list was null!");
            }
        }

        public static void CreateHandComments(LetterObj letter)
        {
            /*erzeugt eine anmerkungen über stellen, an denen fremde hände auftauchen.
             die liste handtags wird beim parsen befüllt: jedes mal wenn ein xemelement mit dem localname hand auftaucht, 
             wird es hinzugefügt. das einzige was dann etwas kniffelig ist, ist herauszufinden von wo (zeile/seite) bis 
             wo sich der bereich mit fremder hand erstreckt*/

            MakeHeading(letter.WordDoc, "Zusätze fremder Hand");
            foreach (var hand in letter.HandTags)
            {
                Paragraph para = GetLastPara(letter.WordDoc);
                string firstPage;
                string lastPage = "";
                string firstLine;
                string lastLine = "";
                string isFN = "Fußnote ";

                XElement xLine = GetLineXisIn(hand);
                firstPage = GetPageXisOn(xLine, letter);
                if (!xLine.Attributes("index").Any() || xLine.Attributes("fn").Any())
                {
                    firstLine = isFN;
                }
                else
                {
                    firstLine = xLine.Attribute("index").Value;
                }

                var lastXPage = hand.Descendants().Where(x => !((x as XNode) is XText) && x?.Name?.LocalName == "page").LastOrDefault();
                if (lastXPage != null)
                {
                    lastPage = lastXPage.Attribute("index").Value;
                }

                var lastXLine = hand.Descendants().Where(x => !((x as XNode) is XText) && x?.Name?.LocalName == "line").LastOrDefault();
                if (lastXLine != null && firstLine != isFN)
                {
                    lastLine = lastXLine.Attribute("index").Value;
                }

                if (lastPage != "")
                {
                    firstPage = firstPage + "\u2012" + lastPage + "/";
                }
                else
                {
                    firstPage += "/";
                }

                if (lastLine != "")
                {
                    firstLine = firstLine + "\u2012" + lastLine + " ";
                }
                else
                {
                    firstLine += " ";
                }
                var nameIndex = hand.Attribute("ref").Value;
                string descript = "";
                if (Letters.HandPersons.ContainsKey(nameIndex))
                {
                    descript = Letters.HandPersons[nameIndex].Name;
                }
                else
                {
                    Logger.Out("Brief " + letter.Autopsic + " enthält die Handreferenz " + nameIndex + "\ndie nicht in Names enthalten ist");
                }
                var pageLine = new Run(new Text(firstPage + firstLine) { Space = SpaceProcessingModeValues.Preserve });
                BoldRun(pageLine);
                SmallFont(pageLine, MiddleFontSize);
                var kommpara = para.InsertAfterSelf(new Paragraph(pageLine, new Run(new Text(descript))));
                ApplyParaStyle(kommpara, "kommentar");
            }
        }

        public static XElement GetLineXisIn(XElement xelem)
        {
            /*XElement tag = xelem;
            while (tag.ElementsBeforeSelf().Where(x => !((x as XNode) is XText) && x?.Name?.LocalName == "line").Count() == 0)
            {
                foreach (var xel in tag.DescendantNodes)

                tag = tag.Parent;
            }
            XElement lineTag = tag.ElementsBeforeSelf()?.Last(x => x.Name.LocalName == "line"); */
            XElement lineTag;
            if (xelem.ElementsBeforeSelf().Where(x => !((x as XNode) is XText) && x?.Name?.LocalName == "line").Count() == 0)
            {
                var tag = xelem;
                while (!tag.ElementsBeforeSelf().Where(x => !((x as XNode) is XText) && x?.Name?.LocalName == "line").Any())
                {
                    tag = tag.Parent;
                }
                /*bool children = false;
                bool siblings = false;
                var tag = xelem;
                while (!children && !siblings)
                {
                    tag = tag.Parent;
                    if(tag.Elements().Where(x => !((x as XNode) is XText) && x?.Name?.LocalName == "line").Any())
                    {
                        children = true;
                    }
                    else if (tag.ElementsBeforeSelf().Where(x => !((x as XNode) is XText) && x?.Name?.LocalName == "line").Any())
                    {
                        siblings = true;
                    }
                }
                lineTag = tag.Elements()?.Last(x => x.Name.LocalName == "line");*/
                lineTag = tag.ElementsBeforeSelf().Where(x => !((x as XNode) is XText) && x?.Name?.LocalName == "line").Last();
            }
            else
            {
                lineTag = xelem.ElementsBeforeSelf().Where(x => !((x as XNode) is XText) && x?.Name?.LocalName == "line").Last();
            }
            return lineTag;
        }

        static public Style CreateParaStyle(WordprocessingDocument wordDoc, string styleid, string stylename, string nextStyle = "", string basedOn = "", string einzugwert = "0", string justification = "")
        {
            // erzeugt Absatzvorlage. damit die zahl der Argumente nich zu unübersichtlich wird, muss man ein paar einstellungen extern bestimmen ...
            Styles stylesRoot = wordDoc.MainDocumentPart.StyleDefinitionsPart.Styles;

            Style style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = styleid,
                CustomStyle = true,
                Default = false
            };

            Aliases alias = new Aliases() { Val = stylename };
            StyleName stylenameprop = new StyleName() { Val = stylename };
            UnhideWhenUsed unhidewhenused = new UnhideWhenUsed() { Val = OnOffOnlyValues.On };
            SemiHidden semihidden = new SemiHidden() { Val = OnOffOnlyValues.Off };
            StyleHidden stylehidden = new StyleHidden() { Val = OnOffOnlyValues.Off };
            PrimaryStyle primarystyle = new PrimaryStyle() { Val = OnOffOnlyValues.On };
            ParagraphProperties paragraphProperties = new ParagraphProperties(
                new SpacingBetweenLines() { After = "0", Before = "0", Line = LineHight, LineRule = LineSpacingRuleValues.Exact },
                new Indentation() { Left = einzugwert }
            );

            if (justification != null)
            {

                Justification justify = new Justification();
                if (justification == "left")
                {
                    justify.Val = JustificationValues.Left;
                }
                else if (justification == "right")
                {
                    justify.Val = JustificationValues.Right;
                }
                else if (justification == "center")
                {
                    justify.Val = JustificationValues.Center;
                }
                else
                {
                    justify.Val = JustificationValues.Left;
                }
                paragraphProperties.Append(justify);
            }

            style.Append(alias);
            style.Append(stylenameprop);
            style.Append(unhidewhenused);
            style.Append(semihidden);
            style.Append(stylehidden);
            style.Append(primarystyle);
            style.Append(paragraphProperties);


            if (basedOn == "")
            {

                BasedOn basedon = new BasedOn() { Val = "stumpf" };
                style.Append(basedon);
            }
            else
            {
                BasedOn basedon = new BasedOn() { Val = basedOn };
                style.Append(basedon);
            }

            if (nextStyle == "")
            {
                NextParagraphStyle nextParaStyle = new NextParagraphStyle() { Val = "einzug" };
                style.Append(nextParaStyle);
            }
            else
            {
                NextParagraphStyle nextParaStyle = new NextParagraphStyle() { Val = nextStyle };
                style.Append(nextParaStyle);
            }


            // Schriftgröße und alles was primär die Runs betriff muss in den Stylerunproperties bestimmt werden
            StyleRunProperties runStyle = new StyleRunProperties();
            RunFonts font = new RunFonts() { Ascii = NormalFont, HighAnsi = NormalFont, ComplexScript = NormalFont };
            FontSize fontSize = new FontSize() { Val = FontSize };
            runStyle.Append(font);
            runStyle.Append(fontSize);
            style.Append(runStyle);
            stylesRoot.Append(style);
            return style;
        }
        #endregion
    }

}
