namespace HaWeb.Models;

public class SyntaxCheckModel {
    public string File { get; private set; }
    public string Commit { get; private set; }
    public List<SyntaxError>? Errors { get; private set; }

    public SyntaxCheckModel(string file, string commithash) {
        File = file;
        Commit = commithash;
    }

    public void Log(int? line, int? column, string msg) {
        if (String.IsNullOrWhiteSpace(msg)) return;
        if (Errors == null) Errors = new();
        // var prefix = DateTime.Now.ToLongDateString() + ": ";
        Errors.Add(new SyntaxError(line, column, msg));
    }

    public void SortErrors() {
        if (Errors != null)
            Errors = Errors.OrderBy(x => x.Line).ToList();
    }

    public void ResetLog() {
        Errors = null;
    }
}

