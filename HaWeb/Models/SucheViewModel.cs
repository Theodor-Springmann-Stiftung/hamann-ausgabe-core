namespace HaWeb.Models;
using HaDocument.Models;

public enum SearchResultType {
    Success,
    OutOfBounds,
    NotFound,
    InvalidSearchTerm
}

public enum SearchType {
    Letters,
    Register,
    Science
}

public class SucheViewModel {
    public List<(int Year, List<BriefeMetaViewModel> LetterList)>? Letters { get; private set; }
    public List<CommentModel>? Comments { get; private set; }
    public Dictionary<string, List<(Marginal, string)>>? Marginals { get; private set; }

    public int Count { get; private set; }
    public int ActivePage { get; private set; }
    public bool? IncludeComments { get; private set; }
    public string ActiveSearch { get; private set; }
    public List<string>? AvailablePages { get; private set; }
    public Dictionary<string, List<SearchResult>>? SearchResults { get; private set; }
    public SearchResultType SearchResultType { get; private set; }
    public SearchType SearchType { get; private set; }

    public SucheViewModel(
        SearchType searchType,
        SearchResultType searchResultType,
        bool? includeComments,
        int activePage,
        List<string>? availablePages,
        string activeSearch,
        Dictionary<string, List<SearchResult>>? searchResults,
        List<(int Year, List<BriefeMetaViewModel> LetterList)>? letters,
        List<CommentModel>? comments,
        Dictionary<string, List<(Marginal, string)>>? marginals
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
        Comments = comments;
        Marginals = marginals;
        IncludeComments = includeComments;
    }
}