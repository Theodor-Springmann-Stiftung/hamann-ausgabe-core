namespace HaWeb.Models;
using System.Web;

public class BriefeViewModel {
    public string Id { get; private set; }
    public string Index { get; private set; }
    public BriefeMetaViewModel MetaData { get; private set; }
    public string? DefaultCategory { get; set; }

    private List<(string, string, string, string, string, string)>? _ParsedEdits;
    private List<(string, string, string, string, string)>? _ParsedHands;
    private List<(string Category, List<Text>)>? _Texts;
    
    public List<(string Category, List<Text>)>? Texts {
        get => _Texts;
        set {
            if (value != null)
                _Texts = value.Select(x => (
                    x.Item1,
                    x.Item2
                )).ToList();
            else _Texts = null;
        }
    }

    // From, Until, Reference, Edit, sartpage, startline
    public List<(string ParsedStart, string ParsedEnd, string Preview, string Text, string Page, string Line)>? ParsedEdits {
        get => _ParsedEdits;
        set {
            if (value != null)
                _ParsedEdits = value.Select(x => (
                    HttpUtility.HtmlEncode(x.Item1),
                    HttpUtility.HtmlEncode(x.Item2),
                    x.Item3,
                    x.Item4,
                    HttpUtility.HtmlAttributeEncode(x.Item5),
                    HttpUtility.HtmlAttributeEncode(x.Item6)
                )).ToList();
            else _ParsedEdits = null;
        }
    }

    // From, Until, Person, startpage, startline
    public List<(string ParsedStart, string ParsedEnd, string Person, string Page, string Line)>? ParsedHands {
        get => _ParsedHands;
        set {
            if (value != null)
                _ParsedHands = value.Select(x => (
                    HttpUtility.HtmlEncode(x.Item1),
                    HttpUtility.HtmlEncode(x.Item2),
                    HttpUtility.HtmlEncode(x.Item3),
                    HttpUtility.HtmlAttributeEncode(x.Item4),
                    HttpUtility.HtmlAttributeEncode(x.Item5)
                )).ToList();
            else _ParsedHands = null;
        }
    }

    public BriefeViewModel(string id, string index, BriefeMetaViewModel meta) {
        Id = id;
        Index = index;
        MetaData = meta;
    }
}