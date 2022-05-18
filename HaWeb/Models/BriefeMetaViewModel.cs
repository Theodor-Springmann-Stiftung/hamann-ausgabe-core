namespace HaWeb.Models;
using HaDocument.Models;

public class BriefeMetaViewModel
{
    public Meta Meta { get; private set; }

    public string? ParsedSenders { get; set; }
    public string? ParsedReceivers { get; set; }

    public BriefeMetaViewModel(Meta meta)
    {
        Meta = meta;
    }
}