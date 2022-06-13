namespace HaDocument.Models;
using System;
using System.Collections.Generic;

public class Meta {
    public string Index { get; }
    public string Autopsic { get; }
    public string Date { get; }
    public DateTime Sort { get; }
    public int? Order { get; }
    public string Location { get; }
    public List<string> Senders { get; }
    public List<string> Receivers { get; }
    public bool? hasOriginal { get; }
    public bool? isProofread { get; }
    public bool? isDraft { get; }
    public ZHInfo? ZH { get; }

    public Meta(
        string index,
        string autopsic,
        string date,
        DateTime sort,
        int? order,
        string location,
        List<string> senders,
        List<string> receivers,
        bool? hasOriginal,
        bool? isProofread,
        bool? isDraft,
        ZHInfo? ZH
    ) {
        Index = index;
        Autopsic = autopsic;
        Date = date;
        Sort = sort;
        Order = order;
        Location = location;
        Senders = senders;
        Receivers = receivers;
        this.hasOriginal = hasOriginal;
        this.isProofread = isProofread;
        this.isDraft = isDraft;
        this.ZH = ZH;
    }

}
