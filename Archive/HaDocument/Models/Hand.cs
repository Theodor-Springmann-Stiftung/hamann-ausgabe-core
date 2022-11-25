namespace HaDocument.Models {
    public class Hand : HaModel {
        public string Letter { get; } = "";
        public string Person { get; } = "";
        public string StartPage { get; } = "";
        public string StartLine { get; } = "";
        public string EndPage { get; } = "";
        public string EndLine {get; } = "";

        public Hand(
            string letter,
            string person,
            string startpage,
            string startline,
            string endpage,
            string endline
        ) {
            Letter = letter;
            Person = person;
            StartPage = startpage;
            StartLine = startline;
            EndPage = endpage;
            EndLine = endline;
        }
    }
}