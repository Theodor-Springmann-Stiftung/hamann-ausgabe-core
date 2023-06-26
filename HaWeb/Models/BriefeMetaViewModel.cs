namespace HaWeb.Models;
using HaDocument.Models;
using System.Web;
public class BriefeMetaViewModel {
    public Meta Meta { get; private set; }
    public bool HasMarginals { get; private set; }
    public bool HasText { get; set; } = true;

    private List<(string Sender, string Receiver)>? _SenderReceiver;
    private string? _ParsedZHString;
    private string? _Startline;
    private string? _Startpage;

    public List<(string Sender, string Receiver)>? SenderReceiver {
        get => _SenderReceiver;
        set {
            if (value != null)
                _SenderReceiver = value;
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

    public (BriefeMetaViewModel Model, string)? Next { get; set; }
    public (BriefeMetaViewModel Model, string)? Prev { get; set; }


    public BriefeMetaViewModel(Meta meta, bool hasMarginals) {
        Meta = meta;
        HasMarginals = hasMarginals;
    }
}