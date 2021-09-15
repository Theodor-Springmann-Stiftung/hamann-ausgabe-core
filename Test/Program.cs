using HaDocument.Interfaces;
using HaDocument.Models;
using HaDocument;
using System.Diagnostics;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using HaXMLReader.EvArgs;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            var test = Document.Create(new Options());
            sw.Stop();
            sw.Reset();

            sw.Start();

            var test1 = test.CommentsByCategoryLetter["neuzeit"].OrderBy(x => x.Key);
            foreach (var kv in test1) {
                Console.WriteLine(kv.Key);
            }

            sw.Stop();

            sw.Start();

            var test2 = test.MetasByDate.Where(x => x.Autopsic == "1172").FirstOrDefault();
            if (test2 != null)
                Console.WriteLine(test2.Sort.ToShortDateString());

            sw.Stop();

            sw.Reset();
            
            // Parallel HaDocument
            var bag = new ConcurrentBag<Text>();
            void ReadT(object _, Text text) {
                if (text.Value.ToLower().Contains("winter"))
                    bag.Add(text);
            }
            void simpleFunction(Marginal letter) {
                var x = new HaXMLReader.UTF8StringReader(letter.Element);
                x.Text += ReadT;
                x.Read();
            }
            sw.Start();
            Parallel.ForEach(test.Marginals, (marg) => {
                simpleFunction(marg.Value);
            } );
            sw.Stop();

            sw.Reset();
            
            // HaDocument
            var HaBag = new ConcurrentBag<Text>();
            void HaReadT(object _, Text text) {
                if (text.Value.ToLower().Contains("winter"))
                    HaBag.Add(text);
            }
            void HasimpleFunction(Letter letter) {
                var x = new HaXMLReader.UTF8StringReader(letter.Element);
                x.Text += HaReadT;
                x.Read();
            }
            sw.Start();
            Parallel.ForEach (test.Letters , (letter) => {
                HasimpleFunction(letter.Value);
            });
            sw.Stop();

            sw.Reset();

            // Linq-Test:
            sw.Start();
            var haLinq = XDocument.Load(new Options().HamannXMLFilePath);
            sw.Stop();

            sw.Reset();

            var LinqResults = new ConcurrentBag<XText>();
            sw.Start();

            Parallel.ForEach(haLinq.Root.Element("document").DescendantNodes(), (element) => {
                    if (element is XText) {
                        var text = (XText)element;
                        if (text.Value.ToLower().Contains("winter"))
                            LinqResults.Add(text);
                    }
                }
            );
            
            sw.Stop();

            sw.Reset();
        }
    }

    class Options : IHaDocumentOptions {
        public string HamannXMLFilePath {get;set;} = 
        System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) ? 
        @"/home/simon/repos/Hamann/XML_Aktuell/Hamann.xml" :
        @"L:\source\hamann\XML_Aktuell\Hamann.xml";
        public string[] AvailableVolumes { get; set; } = { "1", "2", "3", "4", "5", "6", "7" };
        public bool NormalizeWhitespace { get; set; } = true;
        public (int, int) AvailableYearRange {get; set; } = (0, 0);

    }
}
