namespace HaWeb.Models;

public class BriefeViewModel
{
    public string Id { get; private set; }
    public string Index { get; private set; }
    public BriefeMetaViewModel MetaData { get; private set; }

    public string? ParsedText { get; set; }
    public string? ParsedLineCount { get; set; }
    public string? ParsedMarginals { get; set; }
    public string? ParsedTradition { get; set; }
    // From, Until, Reference, Edit, sartpage, startline
    public List<(string, string, string, string, string, string)>? ParsedEdits { get; set; }
    // From, Until, Person, startpage, startline
    public List<(string, string, string, string, string)>? ParsedHands { get; set; }
    
    public BriefeViewModel(string id, string index, BriefeMetaViewModel meta)
    {
        Id = id;
        Index = index;
        MetaData = meta;
    }
}