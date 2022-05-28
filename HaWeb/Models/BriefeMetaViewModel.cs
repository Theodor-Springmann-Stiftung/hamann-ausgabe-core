namespace HaWeb.Models;
using HaDocument.Models;

public class BriefeMetaViewModel
{
    public Meta Meta { get; private set; }
    public bool HasMarginals { get; private set; }
    public bool ShowZHData { get; private set; }

    public string? ParsedSenders { get; set; }
    public string? ParsedReceivers { get; set; }
    public string? ParsedZHString { get; set; }
    
    public (BriefeMetaViewModel, string)? Next { get; set; }
    public (BriefeMetaViewModel, string)? Prev { get; set; }


    public BriefeMetaViewModel(Meta meta, bool hasMarginals, bool showZHData)
    {
        Meta = meta;
        HasMarginals = hasMarginals;
        ShowZHData = showZHData;
    }
}