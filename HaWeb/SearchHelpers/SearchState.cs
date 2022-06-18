namespace HaWeb.SearchHelpers;
using System.Text;

public class SearchState : HaWeb.HTMLParser.IState {
    internal string SearchWord;
    internal bool Normalize;
    internal List<(string Page, string Line)>? Results;

    public SearchState(string searchword, bool normalize = false) {
        Normalize = normalize;
        SearchWord = searchword;
    }
    
    public void SetupState() {}
}