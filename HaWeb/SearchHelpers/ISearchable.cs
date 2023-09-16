namespace HaWeb.SearchHelpers;

public interface ISearchable {
    public string ID { get; }
    public string? SearchText { get; }
}