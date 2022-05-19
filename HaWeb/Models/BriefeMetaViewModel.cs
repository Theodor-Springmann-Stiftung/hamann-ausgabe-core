namespace HaWeb.Models;
using HaDocument.Models;

public class BriefeMetaViewModel
{
    public Meta Meta { get; private set; }
    public bool ShowSurroundingLetterLinks { get; private set; }
    public bool ShowPDFButton { get; private set; }

    public string? ParsedSenders { get; set; }
    public string? ParsedReceivers { get; set; }
    
    public (BriefeMetaViewModel, string)? Next { get; set; }
    public (BriefeMetaViewModel, string)? Prev { get; set; }


    public BriefeMetaViewModel(Meta meta)
    {
        Meta = meta;
    }
}