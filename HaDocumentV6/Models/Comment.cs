using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HaDocument.Models{
    public class Comment {
        public string Element { get; } = "";
        public string Index { get; } = "";
        public string Type { get; } = "";
        public string Lemma { get; } = "";
        public string Parent { get; } = "";
        public int Order { get; } = -1;
        public ImmutableSortedDictionary<string, Comment> Kommentare { get; } 

        public Comment(
            string entry,
            string index,
            string type,
            string lemma,
            int order,
            SortedDictionary<string, Comment> subComments,
            string parent=""
        ) {
            Element = entry;
            Index = index;
            Type = type;
            Lemma = lemma;
            Order = order;
            Parent = parent;
            if (subComments != null) 
                Kommentare = ImmutableSortedDictionary.ToImmutableSortedDictionary(subComments);
            else
                Kommentare = null;
        }
    }
}