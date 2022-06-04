namespace HaWeb.Models;
using HaDocument.Models;
using HaDocument.Comparers;
using HaDocument.Interfaces;
using System.Collections.Generic;

public class DocumentSearchResult {
    public Meta MetaData { get; }
    public List<DocumentResult> Results { get; }

    public DocumentSearchResult(Meta meta) {
        MetaData = meta;
        Results = new List<DocumentResult>(4);
    }
}

public class DocumentResult {
    public string PreviewString { get; }
    public string Page { get; }
    public string Line { get; }

    public DocumentResult(string previewstring, string page, string line) {
        PreviewString = previewstring;
        Page = page;
        Line = line;
    }
}

public class LetterComparer : IComparer<DocumentSearchResult> {
    public int Compare(DocumentSearchResult first, DocumentSearchResult second) {
        var cmp = new DefaultComparer();
        return cmp.Compare(first.MetaData, second.MetaData);
    }
}