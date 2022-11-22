namespace HaWeb.Models;
using HaDocument.Models;
using System.Web;
public class BriefeMetaViewModel {
    public Meta Meta { get; private set; }
    public bool HasMarginals { get; private set; }
    public bool HasText { get; set; } = true;

    private string? _ParsedSenders;
    private string? _ParsedReceivers;
    private string? _ParsedZHString;
    private string? _Startline;
    private string? _Startpage;

    public string? ParsedSenders {
        get => _ParsedSenders;
        set {
            if (value != null)
                _ParsedSenders = HttpUtility.HtmlEncode(value);
            else
                _ParsedSenders = value;
        }
    }

    public string? ParsedReceivers {
        get => _ParsedReceivers;
        set {
            if (value != null)
                _ParsedReceivers = HttpUtility.HtmlEncode(value);
            else
                _ParsedReceivers = value;

        }
    }

    public string? ParsedZHString {
        get => _ParsedZHString;
        set {
            if (value != null)
                _ParsedZHString = HttpUtility.HtmlEncode(value);
            else
                _ParsedZHString = value;

        }
    }

    public string? Startline {
        get => _Startline;
        set {
            if (value != null)
                _Startline = HttpUtility.HtmlEncode(value);
            else
                _Startline = value;
        }
    }

    public string? Startpage {
        get => _Startpage;
        set {
            if (value != null)
                _Startpage = HttpUtility.HtmlEncode(value);
            else
                _Startpage = value;
        }
    }

    public (BriefeMetaViewModel, string)? Next { get; set; }
    public (BriefeMetaViewModel, string)? Prev { get; set; }


    public BriefeMetaViewModel(Meta meta, bool hasMarginals) {
        Meta = meta;
        HasMarginals = hasMarginals;
    }
}