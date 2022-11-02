namespace HaWeb.Models;
using HaDocument.Models;
using HaDocument.Comparers;
using HaDocument.Interfaces;
using System.Collections.Generic;

public class SearchResult  {
    public string Search { get; private set; }
    public string Index { get; private set; }
    public string Identifier { get; set; }
    public string? Page { get; set; }
    public string? Line { get; set; }
    public string? Preview { get; set; }

    // TODO:
    public string? ParsedPreview { get; set; }

    public SearchResult(string search, string index) {
        Search = search;
        Index = index;
    }
}