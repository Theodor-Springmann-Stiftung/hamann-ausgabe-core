using System;
using System.Collections.Generic;
using HaXMLReader.EvArgs;

namespace HaDocument.Models {
    public class Meta {
        public string Index { get; } = "";
        public string Autopsic { get; } = "";
        public string Date { get; } = "";
        public DateTime Sort { get; } = new DateTime(1700, 1, 1);
        public int Order { get; } = -1;
        public string Location { get; } = "";
        public List<string> Senders { get; } = null;
        public List<string> Receivers { get; } = null;
        public OptionalBool hasOriginal { get; } = OptionalBool.None;
        public OptionalBool isProofread { get; } = OptionalBool.None;
        public OptionalBool isDraft { get; } = OptionalBool.None;
        public ZHInfo ZH { get; } = null;

        public Meta(
            string index, 
            string autopsic, 
            string date, 
            DateTime sort, 
            int order,
            OptionalBool hasOriginal,
            OptionalBool isProofread,
            OptionalBool isDraft,
            string location,
            List<string> senders,
            List<string> receivers,
            ZHInfo ZH
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
}