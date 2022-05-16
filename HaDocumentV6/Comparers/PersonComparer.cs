using System;
using HaDocument.Models;
using System.Collections.Generic;

namespace HaDocument.Comparers {
    public class PersonComparer : IComparer<Person> {
        public int Compare(Person first, Person second) {
            var cmp = String.Compare(first.Surname, second.Surname);
            if (cmp == 0) cmp = String.Compare(first.Name, second.Name);
            return cmp;
        }
    }
}