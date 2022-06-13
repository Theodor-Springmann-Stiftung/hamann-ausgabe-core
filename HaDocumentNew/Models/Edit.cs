namespace HaDocument.Models;

public class Edit {
    public string StartPage { get; }
    public string StartLine { get; }
    public string EndPage { get; }
    public string EndLine { get; }
    public string Reference { get; }

    public Edit(
        string reference,
        string startpage,
        string startline,
        string endpage,
        string endline
    ) {
        StartPage = startpage;
        StartLine = startline;
        EndPage = endpage;
        EndLine = endline;
        Reference = reference;
    }
}