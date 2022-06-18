namespace HaWeb.Models;
using HaDocument.Models;

public class SucheViewModel {
    public List<(int Year, List<BriefeMetaViewModel> LetterList)>? Letters { get; private set; }
    public int Count { get; private set; }
    public int ActiveYear { get; private set; }
    public List<(int StartYear, int EndYear)>? AvailableYears { get; private set; }
    public string? ActivePerson { get; set; }
    public List<(string Key, string Name)>? AvailablePersons { get; private set; }
    public List<(string Volume, List<string> Pages)>? AvailablePages { get; private set; }
    public string? ActiveVolume { get; private set; }
    public string? ActivePage { get; private set; }
    public string? ActiveSearch { get; private set; }
    public Dictionary<string, List<SearchResult>>? SearchResults { get; private set; }

    public SucheViewModel(
        List<(int Year, List<BriefeMetaViewModel> LetterList)>? letters,
        int activeYear,
        List<(int StartYear, int EndYear)>? availableYears,
        List<(string Key, string Name)>? availablePersons,
        List<(string Volume, List<string> Pages)>? availablePages,
        string? activeVolume,
        string? activePage,
        string? activeSearch,
        Dictionary<string, List<SearchResult>>? searchResults
    ) {
        Letters = letters;
        if (letters != null)
            Count = letters.Select(x => x.LetterList.Count).Aggregate(0, (x, y) => { x += y; return x; });
        else
            Count = 0;
        ActiveYear = activeYear;
        AvailableYears = availableYears;
        AvailablePersons = availablePersons;
        AvailablePages = availablePages;
        ActiveVolume = activeVolume;
        ActivePage = activePage;
        ActiveSearch = activeSearch;
        SearchResults = searchResults;
    }
}