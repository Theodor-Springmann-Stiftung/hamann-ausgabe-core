namespace HaWeb.Models;

public class SyntaxError {
    public string Message { get; private set; }
    public int? Line { get; set; }
    public int? Column { get; set; }

    public SyntaxError(int? line, int? column, string message) {
        Line = line;
        Column = column;
        Message = message;
    }
}