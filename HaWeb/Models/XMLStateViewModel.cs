namespace HaWeb.Models;
using HaWeb.XMLParser;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Rendering;

public class XMLStateViewModel {
    // Titel der Seite / Aktiver Präfix
    public string ActiveTitle { get; private set; }
    public IFileInfo? ActiveFile { get; set; }
    public GitState? GitData { get; private set; }
    public bool ValidState { get; private set; }

    // Akuell geladene Dateien
    public List<FileModel>? ManagedFiles { get; private set; }

    // Verfügbare (Gesamt-)Dateien
    public List<IFileInfo>? HamannFiles { get; set; }

    // Syntax-Check-Resultate
    public Dictionary<string, SyntaxCheckModel>? SyntaxCheck { get; set; }

    public XMLStateViewModel(
        string title, 
        GitState? gitData,
        List<IFileInfo>? hamannFiles,
        List<FileModel>? managedFiles,
        bool validState) {
            ActiveTitle = title;
            HamannFiles = hamannFiles;
            ManagedFiles = managedFiles;
            GitData = gitData;
            ValidState = validState;
    }
}