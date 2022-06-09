namespace HaWeb.Models;
using System.Web;

public class RegisterViewModel {
    public string Category { get; private set; }
    public string Id { get; private set; }
    public string Title { get; private set; }
    public bool AllowSendIn { get; private set; }
    public bool AllowSearch { get; private set; }

    private List<(string, string)>? _AvailableCategories;
    private List<(string, string)>? _AvailableSideCategories;

    public string? Search { get; set; } = null;
    public bool? MaxSearch { get; set; } = null;
    public List<CommentModel> ParsedComments { get; private set; }

    // Title, URL
    public List<(string, string)>? AvailableCategories {
        get => _AvailableCategories;
        set {
            if (value != null)
                _AvailableCategories = value.Select(x => (
                    HttpUtility.HtmlEncode(x.Item1),
                    HttpUtility.HtmlAttributeEncode(x.Item2))
                ).ToList();
            else
                _AvailableCategories = null;
        }
    }

    // Title, URL
    public List<(string, string)>? AvailableSideCategories {
        get => _AvailableSideCategories;
        set {
            if (value != null)
                _AvailableSideCategories = value.Select(x => (
                    HttpUtility.HtmlEncode(x.Item1),
                    HttpUtility.HtmlAttributeEncode(x.Item2))
                ).ToList();
            else
                _AvailableSideCategories = null;
        }
    }

    public RegisterViewModel(string category, string id, List<CommentModel> parsedComments, string title, bool allowSendIn, bool allowSearch) {
        this.Category = HttpUtility.HtmlAttributeEncode(category);
        this.Id = HttpUtility.HtmlAttributeEncode(id);
        this.ParsedComments = parsedComments;
        this.Title = HttpUtility.HtmlEncode(title);
        this.AllowSendIn = allowSendIn;
        this.AllowSearch = allowSearch;
    }
}