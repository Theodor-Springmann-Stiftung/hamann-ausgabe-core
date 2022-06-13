namespace HaDocument.Models;
using System;
using System.Collections.Generic;
using System.Text;

public class ZHInfo {
    public bool? alternativeLineNumbering { get; }
    public bool? dateChanged { get; }
    public string Volume { get; }
    public string Page { get; }

    public ZHInfo(string Volume, string Page, bool? alternativeLineNumbering, bool? dateChanged) {
        this.alternativeLineNumbering = alternativeLineNumbering;
        this.dateChanged = dateChanged;
        this.Volume = Volume;
        this.Page = Page;
    }
}

