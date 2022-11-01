namespace HaWeb.Models;
using HaDocument.Models;

public enum SearchType {
    Letter,
    Register,
    Marginals
}

public enum SearchResultType {
    Success,
    OutOfBounds,
    NotFound,
    InvalidSearchTerm
}

public class SucheViewModel {
    public List<(int Year, List<BriefeMetaViewModel> LetterList)>? Letters { get; private set; }
    public int Count { get; private set; }
    public int ActivePage { get; private set; }
    public List<string>? AvailablePages { get; private set; }
    public string ActiveSearch { get; private set; }
    public Dictionary<string, List<SearchResult>>? SearchResults { get; private set; }
    public SearchResultType SearchResultType { get; private set; }
    public SearchType SearchType { get; private set; }

    public SucheViewModel(
        SearchType searchType,
        SearchResultType searchResultType,
        int activePage,
        List<string>? availablePages,
        string activeSearch,
        Dictionary<string, List<SearchResult>>? searchResults,
        List<(int Year, List<BriefeMetaViewModel> LetterList)>? letters
    ) {
        Letters = letters;
        if (letters != null)
            Count = letters.Select(x => x.LetterList.Count).Aggregate(0, (x, y) => { x += y; return x; });
        else
            Count = 0;

        SearchType = searchType;
        SearchResultType = searchResultType;
        ActivePage = activePage;
        AvailablePages = availablePages;
        ActiveSearch = activeSearch;
        SearchResults = searchResults;
    }
}