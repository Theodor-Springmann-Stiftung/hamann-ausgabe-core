using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Xml.Linq;

namespace HaEdits
{
    class Program
    {
        static void Main(string[] args)
        {
            var doc = XDocument.Load("../../../briefe.xml", LoadOptions.PreserveWhitespace);
            var document = doc.Root.Element("document");
            string pg = "";
            foreach (var elem in document.Descendants())
            {
                if (elem.Name == "page")
                    pg = elem.Attribute("index").Value;
                if (elem.Name == "structure" && elem.Attribute("ref").Value == "2")
                {
                    if (elem.Descendants().First().Name != "page" )
                    {
                        var ne = new XElement("page");
                        ne.Add(new XAttribute("index", pg.ToString())); ;
                        ne.Add(new XAttribute("autopsic", pg.ToString()));
                        
                        elem.AddFirst(ne);
                        elem.AddFirst("\r\n\t\t\t\t");
                    }
                }
            }
            foreach (var bd in document.Elements("structure"))
            {
                var l = bd.Elements();
                foreach (var lt in l) { lt.Name = "letterText"; lt.Attribute("ref").Remove(); }
                document.Add(l);
            }
            document.Elements("structure").Remove();
            foreach (var e in document.Elements("letterText")) e.AddAfterSelf("\r\n\t\t\t");
            doc.Save("briefeedit.xml", SaveOptions.DisableFormatting);
        }
    }
}
