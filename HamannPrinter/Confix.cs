using System.Collections.Generic;

namespace HamannPrinter
{
    public class Confix
    {
        public bool? LettersDocx { get; set; }
        public bool? VolumeDocx { get; set; }
        public bool? RegisterDocx { get; set; }
        public string HamannXmlPath { get; set; }
        public string OutputPath { get; set; }
        public (int, int) Years { get; set; }
        public string Editionsrichtlinien { get; set; }
    }
}
