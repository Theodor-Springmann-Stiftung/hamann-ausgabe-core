namespace HaWeb.Models;

public class SyntaxCheckModel {
    public string File { get; private set; }
    public List<SyntaxError>? Errors { get; private set; }

    public SyntaxCheckModel(string file) {
        File = file;
    }

    public void Log(int? line, int? column, string msg) {
        if (String.IsNullOrWhiteSpace(msg)) return;
        if (Errors == null) Errors = new();
        // var prefix = DateTime.Now.ToLongDateString() + ": ";
        Errors.Add(new SyntaxError(line, column, msg));
    }

    public void ResetLog() {
        Errors = null;
    }
}

