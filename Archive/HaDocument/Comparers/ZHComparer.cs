using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaDocument.Models;

namespace HaDocument.Comparers
{
    public class ZHComparer : IComparer<Meta>
    {
        public int Compare(Meta first, Meta second)
        {
            var firstNumber = 0;
            var secondNumber = 0;
            Int32.TryParse(first.Index, out firstNumber);
            Int32.TryParse(second.Index, out secondNumber);
            return firstNumber.CompareTo(secondNumber);

            //var firstIndex = from c in first.Meta.Autopsic
            //                    where char.IsDigit(c)
            //                    select c;
            //var secondIndex = from c in second.Meta.Autopsic
            //                 where char.IsDigit(c)
            //                 select c;
            //int firstNumber = 0;
            //int secondNumber = 0;
            //Int32.TryParse(String.Join("", firstIndex), out firstNumber);
            //Int32.TryParse(String.Join("", secondIndex), out secondNumber);
            //if (firstNumber.CompareTo(secondNumber) != 0)
            //    return firstNumber.CompareTo(secondNumber);
            //var firstChar = from c in first.Meta.Autopsic
            //            where char.IsMeta(c)
            //            select c;
            //var secondChar = from c in first.Meta.Autopsic
            //                 where char.IsMeta(c)
            //                 select c;
            //return String.Compare(String.Join("", firstChar), String.Join("", secondChar));
        }
    }
}
