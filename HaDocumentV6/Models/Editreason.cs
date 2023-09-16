using System.Xml.Linq;

namespace HaDocument.Models {
    public class Editreason {
        public string Index { get; } = "";
        public string Element { get; } = "";
        public string Letter { get; } = "";
        public string StartPage { get; } = "";
        public string StartLine { get; } = "";
        public string EndPage { get; } = "";
        public string EndLine { get; } = "";
        public string Reference { get; } = "";
        
        public Editreason(
            string index,
            string element,
            string letter = "",
            string startpage = "",
            string startline = "",
            string endpage = "",
            string endline = "",
            string reference = ""
        ) {
            Index = index;
            Element = element;
            Letter = letter;
            StartPage = startpage;
            StartLine = startline;
            EndPage = endpage;
            EndLine = endline;
            Reference = reference;
        }

        public static Editreason? FromXElement(XElement element) {
            throw new NotImplementedException("We need two Elements for editreason");
        }
    }
}