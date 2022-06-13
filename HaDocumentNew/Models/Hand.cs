namespace HaDocument.Models;

public class Hand {
    public string Letter { get; }
    public string Reference { get; }
    public string StartPage { get; }
    public string StartLine { get; }
    public string EndPage { get; }
    public string EndLine {get; }

    public Hand(
        string letter,
        string reference,
        string startpage,
        string startline,
        string endpage,
        string endline
    ) {
        Letter = letter;
        Reference = reference;
        StartPage = startpage;
        StartLine = startline;
        EndPage = endpage;
        EndLine = endline;
    }
}
