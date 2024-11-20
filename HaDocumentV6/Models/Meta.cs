using System;
using System.Collections.Generic;
using HaXMLReader.EvArgs;

namespace HaDocument.Models {
    public class AdditionalDates {
        public DateTime? NotBefore { get; } = null;
        public DateTime? NotAfter { get; } = null;
        public DateTime? From { get; } = null;
        public DateTime? To { get; } = null;
        public string? Cert { get; } = null;


        public AdditionalDates(
            DateTime? notBefore,
            DateTime? notAfter,
            DateTime? from,
            DateTime? to,
            string? cert
        ) {
            NotBefore = notBefore;
            NotAfter = notAfter;
            From = from;
            To = to;
            Cert = cert;
        }
    }

    public class Meta {
        public string ID { get; } = "";
        public string Date { get; } = "";
        public DateTime Sort { get; } = new DateTime(1700, 1, 1);
        public AdditionalDates? AdditionalDates { get; } = null;
        public int Order { get; } = -1;
        public string Location { get; } = "";
        public List<string>? Senders { get; } = null;
        public List<string>? Receivers { get; } = null;
        public bool? hasOriginal { get; }
        public bool? isProofread { get; }
        public bool? isDraft { get; }
        public ZHInfo? ZH { get; } = null;

        public Meta(
            string id,
            string date,
            DateTime sort,
            int order,
            bool? hasOriginal,
            bool? isProofread,
            bool? isDraft,
            string location,
            List<string> senders,
            List<string> receivers,
            ZHInfo? ZH,
            AdditionalDates? additionalDates = null
        ) {
            ID = id;
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
            this.AdditionalDates = additionalDates;
        }

    }
}
