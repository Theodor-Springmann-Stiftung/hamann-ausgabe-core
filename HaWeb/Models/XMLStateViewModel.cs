namespace HaWeb.Models;
using HaWeb.XMLParser;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Rendering;

public class XMLStateViewModel {
    // Titel der Seite / Aktiver Präfix
    public string ActiveTitle { get; private set; }
    public IFileInfo? ActiveFile { get; set; }
    public (DateTime PullTime, string Hash)? GitData { get; private set; }
    public bool ValidState { get; private set; }

    // Verfügbare Datei-Typen
    public List<IXMLRoot>? AvailableRoots { get; private set; }

    // Akuell geladene Dateien
    public List<FileModel>? ManagedFiles { get; private set; }

    // Verfügbare (Gesamt-)Dateien
    public List<IFileInfo>? HamannFiles { get; set; }

    // Syntax-Check-Resultate
    public Dictionary<string, SyntaxCheckModel>? SyntaxCheck { get; set; }

    public XMLStateViewModel(
        string title, 
        (DateTime PullTime, string Hash)? gitData,
        List<IXMLRoot>? roots,
        List<IFileInfo>? hamannFiles,
        List<FileModel>? managedFiles,
        bool validState) {
            ActiveTitle = title;
            AvailableRoots = roots;
            HamannFiles = hamannFiles;
            ManagedFiles = managedFiles;
            GitData = gitData;
            ValidState = validState;
    }
}