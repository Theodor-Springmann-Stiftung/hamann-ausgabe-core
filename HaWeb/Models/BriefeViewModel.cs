namespace HaWeb.Models;
using System.Web;

public class BriefeViewModel {
    public string Id { get; private set; }
    public string Index { get; private set; }
    public BriefeMetaViewModel MetaData { get; private set; }

    private List<(string, string, string)>? _ParsedMarginals;
    private List<(string, string, string, string, string, string)>? _ParsedEdits;
    public List<(string, string, string, string, string)>? _ParsedHands;

    public string? ParsedText { get; set; }
    public string? ParsedTradition { get; set; }
    public bool MinWidth { get; set; } = false;
    public bool MinWidthTrad { get; set; } = false;

    // From, Until, Reference, Edit, sartpage, startline
    public List<(string, string, string, string, string, string)>? ParsedEdits {
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
            else
                _ParsedEdits = null;
        }
    }

    // From, Until, Person, startpage, startline
    public List<(string, string, string, string, string)>? ParsedHands {
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
            else
                _ParsedHands = null;
        }
    }

    // Page, Line, Element
    public List<(string, string, string)>? ParsedMarginals {
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

    public BriefeViewModel(string id, string index, BriefeMetaViewModel meta) {
        Id = id;
        Index = index;
        MetaData = meta;
    }
}