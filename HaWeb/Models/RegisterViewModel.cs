namespace HaWeb.Models;
using HaDocument.Models;

public class RegisterViewModel {
    public string Category { get; set; } = "";
    public string Id { get; set; } = "";
    public string Search { get; set; } = "";
    public bool MaxSearch { get; set; } = false;
    // TODO: no null-checks in the Page Logic
    public List<CommentModel>? Comments { get; set; } = null;
    public List<(string, string)>? AvailableCategories { get; set; } = null;
    public List<(string, string)>? AvailableSideCategories { get; set; } = null;
}