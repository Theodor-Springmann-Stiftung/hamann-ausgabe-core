namespace HaWeb.Models;

public class SyntaxCheckModel {
    public string Prefix { get; private set; }
    public List<SyntaxError>? Errors { get; set; }
    public List<SyntaxError>? Warnings { get; set; }

    public SyntaxCheckModel(string prefix) {
        Prefix = prefix;
    }
}

public class SyntaxError {
    public string Message { get; private set; }
    public string? File { get; set; }
    public string? Line { get; set; }
    public string? Column { get; set; }

    public SyntaxError(string message) {
        Message = message;
    }
}