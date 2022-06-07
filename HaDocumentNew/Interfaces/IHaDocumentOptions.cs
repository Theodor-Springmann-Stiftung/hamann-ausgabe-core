namespace HaDocument.Interfaces;

public interface IHaDocumentOptions {
    string HamannXMLFilePath { get; set; }
    string[] AvailableVolumes { get; set; }
    bool NormalizeWhitespace { get; set; }
    (int, int) AvailableYearRange { get; set; }
}
