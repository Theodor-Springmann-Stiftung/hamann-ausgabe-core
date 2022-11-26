namespace HaWeb.Models;
using System.Web;

public class Text {
    public string Id { get; private set; }
    public string Number { get; private set; }
    public bool MinWidth { get; private set; }
    public string Category { get; private set; }
    public string? ParsedText { get; set; }
    
    private string? _Title;
    private List<(string, string, string)>? _ParsedMarginals;

    public List<(string Page, string Line, string Text)>? ParsedMarginals {
        get => _ParsedMarginals;
        set {
            if (value != null)
                _ParsedMarginals = value.Select(x => (
                    HttpUtility.HtmlEncode(x.Item1),
                    HttpUtility.HtmlEncode(x.Item2),
                    x.Item3
                )).ToList();
            else
                _ParsedMarginals = null;
        }
    }

    public string? Title {
        get => _Title;
        set {
            if (!String.IsNullOrWhiteSpace(value)) {
                _Title = value;
            } else _Title = null;
        }
    }

    public Text(string id, string number, string category, bool minwidth) {
        Id = id;
        Number = number;
        Category = category;
        MinWidth = minwidth;
    }
}