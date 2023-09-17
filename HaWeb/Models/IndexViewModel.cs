namespace HaWeb.Models;

using System.Collections.Immutable;
using HaDocument.Models;

public class IndexViewModel {
    public List<(int Year, List<BriefeMetaViewModel> LetterList)>? Letters { get; private set; }
    public int Count { get; private set; }
    public int ActiveYear { get; private set; }
    public List<(int StartYear, int EndYear)>? AvailableYears { get; private set; }
    public string? ActivePerson { get; set; }
    public CommentModel? PersonComment { get; set; }
    public List<string>? AvailableLetters { get; private set; }
    public List<(string Key, string Name)>? AvailablePersons { get; private set; }
    public Dictionary<string, List<string>>? AvailablePages { get; private set; }
    public string? ActiveVolume { get; private set; }
    public string? ActivePage { get; private set; }
    public string EndYear { get; private set; }
    public string EndPageString { get; private set; }



    public IndexViewModel(
        List<(int Year, List<BriefeMetaViewModel> LetterList)>? letters,
        int activeYear,
        string endYear,
        string endPageString,
        List<(int StartYear, int EndYear)>? availableYears,
        List<(string Key, string Name)>? availablePersons,
        Dictionary<string, List<string>>? availablePages,
        List<string>? availableLetters,
        string? activeVolume,
        string? activePage,
        string? activePerson
    ) {
        Letters = letters;
        if (letters != null)
            Count = letters.Select(x => x.LetterList.Count).Aggregate(0, (x, y) => { x += y; return x; });
        else
            Count = 0;
        AvailableLetters = availableLetters;
        ActiveYear = activeYear;
        AvailableYears = availableYears;
        AvailablePersons = availablePersons;
        AvailablePages = availablePages;
        ActiveVolume = activeVolume;
        ActivePage = activePage;
        ActivePerson = activePerson;
        EndYear = endYear;
        EndPageString = endPageString;
    }
}