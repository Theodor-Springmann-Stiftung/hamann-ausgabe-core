namespace HaWeb.FileHelpers;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;
using System.Xml;

public static class XDocumentFileHelper {

    private readonly static XmlReaderSettings _Settings = new XmlReaderSettings()
    {
        CloseInput = true,
        CheckCharacters = false,
        ConformanceLevel = ConformanceLevel.Fragment,
        IgnoreComments = true,
        IgnoreProcessingInstructions = true,
        IgnoreWhitespace = false
    };

    public static async Task<XDocument?> ProcessStreamedFile(byte[] bytes, ModelStateDictionary modelState) {
        try
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var xmlreader = XmlReader.Create(stream, _Settings))
                {
                    return XDocument.Load(xmlreader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

                }
            }
        }
        catch (Exception ex)
        {
            modelState.AddModelError("Error", $"Kein gültiges XML-Dokument geladen. Error: {ex.Message}");
        }

        return null;
    }
}