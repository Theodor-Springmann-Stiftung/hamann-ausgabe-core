namespace HaWeb.Models;
using HaDocument.Models;

public class RegisterViewModel {
    public string Category { get; private set; }
    public string Id { get; private set; }
    public string Title { get; private set;  }

    public string? Search { get; set; } = null;
    public bool? MaxSearch { get; set; } = null;
    // TODO: no null-checks in the Page Logic
    public List<CommentModel> ParsedComments { get; private set; }
    public List<(string, string)>? AvailableCategories { get; set; } = null;
    public List<(string, string)>? AvailableSideCategories { get; set; } = null;

    public RegisterViewModel(string category, string id, List<CommentModel> parsedComments, string title) {
        this.Category = category;
        this.Id = id;
        this.ParsedComments = parsedComments;
        this.Title = title;
    }
}