using System;
using System.Collections.Generic;
using System.Text;

namespace HaDocument.Models
{

    public class ZHInfo
    {
        public bool alternativeLineNumbering { get; } = false;
        public bool dateChanged { get; } = false;
        public string Volume { get; } = "";
        public string Page { get; } = "";

        public ZHInfo(bool alternativeLineNumbering, bool dateChanged, string Volume, string Page) {
            this.alternativeLineNumbering = alternativeLineNumbering;
            this.dateChanged = dateChanged;
            this.Volume = Volume;
            this.Page = Page;
        }
    }
}
