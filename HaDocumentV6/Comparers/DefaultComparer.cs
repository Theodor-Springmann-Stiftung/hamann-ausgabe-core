using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaDocument.Models;

namespace HaDocument.Comparers
{
    public class DefaultComparer : IComparer<Meta>
    {
        public int Compare(Meta first, Meta second)
        {
            if (first.Sort != second.Sort)
                return System.DateTime.Compare(first.Sort, second.Sort);
            else if (first.Order != second.Order)
                return first.Order.CompareTo(second.Order);
            else
                return String.Compare(first.ID, second.ID);
        }
    }
}
