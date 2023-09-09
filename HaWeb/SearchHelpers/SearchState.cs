namespace HaWeb.SearchHelpers;
using System.Text;
using HaDocument.Interfaces;

public class SearchState : HaWeb.HTMLParser.IState {
    internal string SearchWord;
    internal string? CurrentIdentifier;
    internal ILibrary? Lib;
    internal bool Normalize;
    internal List<(string Page, string Line, string? Identifier)>? Results;

    public SearchState(string searchword, bool normalize = false, ILibrary? lib = null) {
        Lib = lib;
        Normalize = normalize;
        SearchWord = searchword;
    }
    
    public void SetupState() {}
}