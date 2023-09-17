using System.Collections.Generic;
using System.Collections;
using System.Linq;
using HaDocument.Models;

namespace HaDocument.Comparers {
    public class CommentComparer : IComparer<Comment> {
        public int Compare(Comment first, Comment second) {
            if (first.Order.HasValue && second.Order.HasValue)
                return first.Order.Value.CompareTo(second.Order.Value);
            else if (first.Order.HasValue)
                return 1;
            else if (second.Order.HasValue)
                return -1;
            else
                return 0;
        }
    }
}