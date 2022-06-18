namespace HaWeb.SearchHelpers;

public class SeachableItem : ISearchable {
    public string Index { get; private set; }
    public string SearchText { get; private set; }

    public SeachableItem(string index, string searchtext) {
        this.Index = index;
        this.SearchText = searchtext;
    }
}