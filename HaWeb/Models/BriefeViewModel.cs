namespace HaWeb.Models;

public class BriefeViewModel
{
    public string Id { get; private set; }
    public string Index { get; private set; }
    public BriefeMetaViewModel Meta { get; private set; }

    public string? ParsedText { get; set; }
    public string? ParsedLineCount { get; set; }
    public string? ParsedMarginals { get; set; }
    public string? ParsedTradition { get; set; }
    public List<string>? ParsedEdits { get; set; }
    public List<string>? ParsedHands { get; set; }
    
    public (BriefeMetaViewModel, string)? Next { get; set; }
    public (BriefeMetaViewModel, string)? Prev { get; set; }

    public BriefeViewModel(string id, string index, BriefeMetaViewModel meta)
    {
        Id = id;
        Index = index;
        Meta = meta;
    }
}