using System;
using HaXMLReader.EvArgs;
using System.Collections.Generic;

namespace HaDocument.Models {
    public abstract class HaModel {
        protected static List<(Func<Tag, bool>, Action<Tag>)> FieldActions = null;

        internal static void AddAction(Func<Tag, bool> If, Action<Tag> Then) {
            if (If == null || Then == null) throw new ArgumentNullException();
            if (FieldActions == null) FieldActions = new List<(Func<Tag, bool>, Action<Tag>)>();
            FieldActions.Add((If, Then));
        }
    }
}
