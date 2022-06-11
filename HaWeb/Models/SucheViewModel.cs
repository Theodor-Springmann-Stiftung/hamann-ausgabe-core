namespace HaWeb.Models;

public class SucheViewModel {
    public List<(int Year, List<BriefeMetaViewModel> LetterList)> Letters { get; private set; }
    public int Count { get; private set; }
    public int ActiveYears { get; private set; }
    public List<(int StartYear, int EndYear)> AvailableYears { get; private set; }

    public SucheViewModel(List<(int Year, List<BriefeMetaViewModel> LetterList)> letters, int count, int activeYears,  List<(int StartYear, int EndYear)> availableYears) {
        Letters = letters;
        Count = count;
        ActiveYears = activeYears;
        AvailableYears = availableYears;
    }
}