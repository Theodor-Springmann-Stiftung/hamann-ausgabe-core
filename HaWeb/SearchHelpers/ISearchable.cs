namespace HaWeb.SearchHelpers;

public interface ISearchable {
    public string Index { get; }
    public string SearchText { get; }
}