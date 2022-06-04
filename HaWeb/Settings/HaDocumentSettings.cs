namespace HaWeb.Settings;
using HaDocument.Interfaces;

class HaDocumentOptions : IHaDocumentOptions {
    public string HamannXMLFilePath { get; set; } = @"Hamann.xml";
    public string[] AvailableVolumes { get; set; } = { };
    public bool NormalizeWhitespace { get; set; } = true;
    public (int, int) AvailableYearRange {get; set; } = (1751, 1788);
}