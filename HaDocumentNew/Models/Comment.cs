namespace HaDocument.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Linq;

public class Comment {
    public XElement Entry { get; }
    public string Value { get; }
    public string Index { get; }
    public string? Type { get; }
    public string? Lemma { get; }
    public string? Parent { get; }
    public int? Order { get; }
    public ImmutableSortedDictionary<string, Comment>? SubComments { get; }

    public Comment(
        XElement entry,
        string value,
        string index,
        string? type,
        string? lemma,
        int? order,
        SortedDictionary<string, Comment>? subComments,
        string? parent
    ) {
        Value = value;
        Entry = entry;
        Index = index;
        Type = type;
        Lemma = lemma;
        Order = order;
        Parent = parent;
        if (subComments != null)
            SubComments = ImmutableSortedDictionary.ToImmutableSortedDictionary(subComments);
        else
            SubComments = null;
    }
}
