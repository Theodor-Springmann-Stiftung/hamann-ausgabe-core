namespace HaWeb.CMIF;

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using HaDocument.Interfaces;

public class TeiHeader {
    [XmlElement("fileDesc")]
    public FileDesc? FileDesc { get; set; } = new FileDesc();
    [XmlElement("profileDesc")]
    public ProfileDesc? ProfileDesc { get; set; } = new ProfileDesc();
}

public class FileDesc {
    [XmlElement("titleStmt")]
    public TitleStatement? TitleStatement { get; set; } = new TitleStatement();
    [XmlElement("publicationStmt")]
    public PublicationStatement? PublicationStatement { get; set; } = new PublicationStatement();
    [XmlElement("sourceDesc")]
    public SourceDescription? SourceDescription { get; set; } = new SourceDescription();
}

public class TitleStatement {
    [XmlElement("title")]
    public string? Title { get; set; } = "Johann Georg Hammann: Kommentierte Briefausgabe (HKB)";
    [XmlElement("editor")]
    public List<Editor>? Editor { get; set; } = new List<Editor>() {
        new Editor() {
            Email = "keidel@tss-hd.de",
            Name  = "Leonard Keidel",
        },
        new Editor() {
            Email = "reibold@tss-hd.de",
            Name  = "Janina Reibold",
        }
    };
}

public class Editor {
    [XmlElement("email")]
    public string? Email { get; set; }
    [XmlText]
    public string? Name { get; set; }
}

public class PublicationStatement {
    [XmlElement("publisher")]
    public List<Publisher>? Publishers { get; set; } = new List<Publisher>() {
        new Publisher() {
            Reference = new PublisherRef() {
                Target = "https://theodor-springmann-stiftung.de",
                Value = "Theodor-Springmann-Stiftung",
            }
        },
        new Publisher() {
            Reference = new PublisherRef() {
                Target = "https://gs.uni-heidelberg.de",
                Value = "Germanistisches Seminar der Universität Heidelberg",
            }
        }
    };
    [XmlElement("idno")]
    public Identifier? Identifier { get; set; } = new Identifier();
    [XmlElement("date")]
    public Date Date { get; set; } = new Date() {
        When = DateTime.Now.ToString("yyyy-MM-dd"),
    };
    [XmlElement("availability")]
    public Availability? Availability { get; set; } = new Availability();
}

public class Publisher {
    [XmlElement("ref")]
    public PublisherRef? Reference { get; set; }
}

public class PublisherRef {
    [XmlAttribute("target")]
    public string? Target { get; set; }
    [XmlText]
    public string? Value { get; set; }

}

public class Availability {
    [XmlElement("licence")]
    public License? License { get; set; } = new License();
}

public class License {
    [XmlAttribute("target")]
    public string? Target { get; set; } = "https://creativecommons.org/licenses/by/4.0/";
    [XmlText]
    public string? Value { get; set; } = "CC-BY 4.0";
}

public class Identifier {
    [XmlAttribute("type")]
    public string? Type { get; set; } = "url";
    [XmlText]
    public string? Value { get; set; } = "https://hamann-ausgabe.de/HKB/CMIF";
}

public class SourceDescription {
    [XmlElement("bibl")]
    public Bibliography Bibliography { get; set; } = new Bibliography();
}

public class Bibliography {
    [XmlAttribute("type")]
    public string? Type { get; set; } = "online";
    [XmlAttribute("xml:id")]
    public string? Id { get; set; } = "x419f1d82-7f42-4f29-ac00-f02e474ce766";
    [XmlText]
    public string? Text { get; set; } = "Johann Georg Hamann: Kommentierte Briefausgabe. Hg. von Leonard Keidel und Janina Reibold, auf Grundlage der Vorarbeiten Arthur Henkels, unter Mitarbeit von Gregor Babelotzky, Konrad Bucher, Christian Großmann, Carl Friedrich Haak, Luca Klopfer, Johannes Knüchel, Isabel Langkabel und Simon Martens (Heidelberg 2020 ff.) [= HKB]. ";
    [XmlElement("ref")]
    public Reference Reference { get; set; } = new Reference();
}

public class Reference {
    [XmlAttribute("target")]
    public string? Target { get; set; } = "https://hamann-ausgabe.de/HKB";
    [XmlText]
    public string? Value { get; set; } = "https://hamann-ausgabe.de/HKB";
}

public class ProfileDesc {
    [XmlElement("correspDesc")]
    public List<CorrespondenceDescription>? CorrespondenceDescriptions { get; set; }
}

public class CorrespondenceDescription {
    [XmlAttribute("source")]
    public string? Source { get; set; } = "#x419f1d82-7f42-4f29-ac00-f02e474ce766";
    [XmlAttribute("ref")]
    public string? Reference { get; set; }
    [XmlAttribute("key")]
    public string? Key { get; set; }
    [XmlElement("correspAction")]
    public List<CorrespondenceAction>? CorrespondenceActions { get; set; }
}

public class CorrespondenceAction {
    [XmlAttribute("type")]
    public string? Type { get; set; }
    [XmlElement("persName")]
    public List<PersonName>? PersonName { get; set; }
    [XmlElement("placeName")]
    public List<PlaceName>? PlaceName { get; set; }
    [XmlElement("date")]
    public Date? Date { get; set; }
}

public class Date {
    [XmlAttribute("when")]
    public string? When { get; set; }
    [XmlAttribute("notBefore")]
    public string? NotBefore { get; set; }
    [XmlAttribute("notAfter")]
    public string? NotAfter { get; set; }
    [XmlAttribute("from")]
    public string? From { get; set; }
    [XmlAttribute("to")]
    public string? To { get; set; }
    [XmlAttribute("cert")]
    public string? Cert { get; set; }
    [XmlText]
    public string? Value { get; set; }

}

public class PersonName {
    [XmlAttribute("evidence")]
    public string? Evidence { get; set; }
    [XmlAttribute("ref")]
    public string? Reference { get; set; }
    [XmlText]
    public string? Name { get; set; }
}

public class PlaceName {
    [XmlAttribute("ref")]
    public string? Reference { get; set; }
    [XmlText]
    public string? Name { get; set; }
}

public class TeiText {
    [XmlElement("body")]
    public TextBody? Body { get; set; } = new TextBody();
}

public class TextBody {
    [XmlElement("p")]
    public string? Paragraph { get; set; } = "";
}

[XmlRoot(ElementName = "TEI", Namespace = "http://www.tei-c.org/ns/1.0")]
public class TeiDocument {
    [XmlElement("teiHeader")]
    public TeiHeader? TeiHeader { get; set; } = new TeiHeader();
    [XmlElement("text")]
    public TeiText? Text { get; set; } = new TeiText();

    public TeiDocument() { }

    public TeiDocument(ILibrary lib) {
        var UNKNOWN_URL = "http://correspSearch.net/unknown";
        var UNKNOWN_TEXT = "Unbekannt";
        var LETTER_URL = "https://hamann-ausgabe.de/HKB/Briefe/";
        var DATE_OUTPUT = "yyyy-MM-dd";
        List<CorrespondenceDescription> cds = new List<CorrespondenceDescription>();
        foreach (var meta in lib.Metas.Values) {
            if (lib.Letters.Where(x => x.Key == meta.ID).Count() == 0) continue;
            var cd = new CorrespondenceDescription();
            cd.Reference = LETTER_URL + meta.ID;
            cd.Key = meta.ID;
            var sent = new CorrespondenceAction();
            sent.Type = "sent";
            var d = new Date();
            if (meta.AdditionalDates != null) {
                d.NotBefore = meta.AdditionalDates.NotBefore != null ? meta.AdditionalDates.NotBefore.Value.ToString(DATE_OUTPUT) : null;
                d.NotAfter = meta.AdditionalDates.NotAfter != null ? meta.AdditionalDates.NotAfter.Value.ToString(DATE_OUTPUT) : null;
                d.From = meta.AdditionalDates.From != null ? meta.AdditionalDates.From.Value.ToString(DATE_OUTPUT) : null;
                d.To = meta.AdditionalDates.To != null ? meta.AdditionalDates.To.Value.ToString(DATE_OUTPUT) : null;
                d.Cert = !string.IsNullOrWhiteSpace(meta.AdditionalDates.Cert) ? meta.AdditionalDates.Cert.ToString() : null;
            }
            if (d.NotBefore == null && d.NotAfter == null && d.From == null && d.To == null) {
                d.When = meta.Sort.ToString(DATE_OUTPUT);
            }
            if (!string.IsNullOrWhiteSpace(meta.Date)) {
                var dta = meta.Date.Split(',');
                var ndate = "";
                if (dta.Length > 1) {
                    ndate = string.Join(',', dta.Skip(1)).Trim();
                }
                else {
                    ndate = dta[0].Trim();
                }
                d.Value = ndate;
            }
            sent.Date = d;
            if (!string.IsNullOrWhiteSpace(meta.Location)) {
                if (lib.Locations.ContainsKey(meta.Location)) {
                    var p = new PlaceName();
                    p.Name = lib.Locations[meta.Location].Name;
                    if (lib.Locations[meta.Location].Reference != null && !string.IsNullOrWhiteSpace(lib.Locations[meta.Location].Reference)) {
                        p.Reference = lib.Locations[meta.Location].Reference;
                    }
                    sent.PlaceName = new List<PlaceName>() { p };
                }
            }
            if (meta.Senders != null && meta.Senders.Count() > 0) {
                sent.PersonName = new List<PersonName>();
                foreach (var sender in meta.Senders) {
                    var pn = new PersonName();
                    if (sender == "-1") {
                        pn.Name = UNKNOWN_TEXT;
                        pn.Reference = UNKNOWN_URL;
                    }
                    else if (lib.Persons.ContainsKey(sender)) {
                        pn.Name = lib.Persons[sender].Name;
                        if (lib.Persons[sender].Reference != null && !string.IsNullOrWhiteSpace(lib.Persons[sender].Reference)) {
                            pn.Reference = lib.Persons[sender].Reference;
                        }
                    }
                    else {
                        pn = null;
                    }
                    if (pn != null) sent.PersonName.Add(pn);
                }
            }
            cd.CorrespondenceActions = new List<CorrespondenceAction>() { sent };
            var recieved = new CorrespondenceAction();
            if (meta.Receivers != null && meta.Receivers.Count() > 0) {
                recieved.PersonName = new List<PersonName>();
                foreach (var sender in meta.Receivers) {
                    var pn = new PersonName();
                    if (sender == "-1") {
                        pn.Name = UNKNOWN_TEXT;
                        pn.Reference = UNKNOWN_URL;
                    }
                    else if (lib.Persons.ContainsKey(sender)) {
                        pn.Name = lib.Persons[sender].Name;
                        if (lib.Persons[sender].Reference != null && !string.IsNullOrWhiteSpace(lib.Persons[sender].Reference)) {
                            pn.Reference = lib.Persons[sender].Reference;
                        }
                    }
                    else {
                        pn = null;
                    }
                    if (pn != null) recieved.PersonName.Add(pn);
                }
            }
            recieved.Type = "received";
            cd.CorrespondenceActions.Add(recieved);
            cds.Add(cd);
        }

        this.TeiHeader.ProfileDesc = new ProfileDesc() { CorrespondenceDescriptions = cds };
    }

}


