using System.Xml;
namespace HaWeb.Settings.XMLCollections;
using HaWeb.Models;
using System.Xml.Linq;


public static class CommentCollectionHelpers {
    public static Func<XElement, string?> GetKey { get; } = (elem) => {
        var index = elem.Attribute("id");
        if (index != null && !String.IsNullOrWhiteSpace(index.Value))
            return index.Value;
        else return null;
    };

    public static IDictionary<string, string>? GetDataFields(XElement element) {
        var res = new Dictionary<string, string>();
        var lemma = element.Descendants("lemma");
        if (!lemma.Any() || String.IsNullOrWhiteSpace(lemma.First().Value)) return null;
        res.Add("lemma", lemma.First().Value);
        return res;
    }

    public static IDictionary<string, ILookup<string, CollectedItem>>? GetLookups(IEnumerable<CollectedItem> items) {
        var res = new Dictionary<string, ILookup<string, CollectedItem>>();
        var lemmas = items.Where(x => !String.IsNullOrWhiteSpace(x.ID));
        if (lemmas != null && lemmas.Any())
            res.Add("lemma", lemmas.ToLookup(x => x.ID.Substring(0, 1).ToUpper()));
        // If we use lemmas
        // var lemmas = items.Where(x => x.Fields != null && x.Fields.ContainsKey("lemma"));
        // if (lemmas != null && lemmas.Any()) 
        //     res.Add("lemma", lemmas.ToLookup(x => x.Fields["lemma"][0].First().ToString()));
        if (!res.Any()) return null;
        return res;
    }
}

public class BibleCommentCollection : HaWeb.XMLParser.IXMLCollection {
    public string Key { get; } = "bible-comments";
    public string[] xPath { get; } = new string[] { "/opus/data/kommentare/kommcat[@value='bibel']/kommentar", "/opus/kommentare/kommcat[@value='bibel']/kommentar" };
    public Func<XElement, string?> GenerateKey { get; } = CommentCollectionHelpers.GetKey;
    public Func<XElement, IDictionary<string, string>?>? GenerateDataFields { get; } = CommentCollectionHelpers.GetDataFields;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, ILookup<string, CollectedItem>>?>? GroupingsGeneration { get; } = CommentCollectionHelpers.GetLookups;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, IEnumerable<CollectedItem>>?>? SortingsGeneration { get; } = null;
    public HaWeb.XMLParser.IXMLCollection[]? SubCollections { get; } = null;
    public bool Searchable { get; } = false;
}

public class EditionCommentCollection : HaWeb.XMLParser.IXMLCollection {
    public string Key { get; } = "edition-comments";
    public string[] xPath { get; } = new string[] { "/opus/data/kommentare/kommcat[@value='editionen']/kommentar", "/opus/kommentare/kommcat[@value='editionen']/kommentar" };
    public Func<XElement, string?> GenerateKey { get; } = CommentCollectionHelpers.GetKey;
    public Func<XElement, IDictionary<string, string>?>? GenerateDataFields { get; } = CommentCollectionHelpers.GetDataFields;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, ILookup<string, CollectedItem>>?>? GroupingsGeneration { get; } = CommentCollectionHelpers.GetLookups;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, IEnumerable<CollectedItem>>?>? SortingsGeneration { get; } = null;
    public HaWeb.XMLParser.IXMLCollection[]? SubCollections { get; } = null;
    public bool Searchable { get; } = true;
}

public class RegisterCommentCollection : HaWeb.XMLParser.IXMLCollection {
    public string Key { get; } = "register-comments";
    public string[] xPath { get; } = new string[] { "/opus/data/kommentare/kommcat[@value='neuzeit']/kommentar", "/opus/kommentare/kommcat[@value='neuzeit']/kommentar" };
    public Func<XElement, string?> GenerateKey { get; } = CommentCollectionHelpers.GetKey;
    public Func<XElement, IDictionary<string, string>?>? GenerateDataFields { get; } = CommentCollectionHelpers.GetDataFields;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, ILookup<string, CollectedItem>>?>? GroupingsGeneration { get; } = CommentCollectionHelpers.GetLookups;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, IEnumerable<CollectedItem>>?>? SortingsGeneration { get; } = null;
    public HaWeb.XMLParser.IXMLCollection[]? SubCollections { get; } = new XMLParser.IXMLCollection[] { new SubCommentCollection() };
    public bool Searchable { get; } = true;
}

public class ForschungCommentCollection : HaWeb.XMLParser.IXMLCollection {
    public string Key { get; } = "forschung-comments";
    public string[] xPath { get; } = new string[] { "/opus/data/kommentare/kommcat[@value='forschung']/kommentar", "/opus/kommentare/kommcat[@value='forschung']/kommentar" };
    public Func<XElement, string?> GenerateKey { get; } = CommentCollectionHelpers.GetKey;
    public Func<XElement, IDictionary<string, string>?>? GenerateDataFields { get; } = CommentCollectionHelpers.GetDataFields;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, ILookup<string, CollectedItem>>?>? GroupingsGeneration { get; } = CommentCollectionHelpers.GetLookups;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, IEnumerable<CollectedItem>>?>? SortingsGeneration { get; } = null;
    public HaWeb.XMLParser.IXMLCollection[]? SubCollections { get; } = null;
    public bool Searchable { get; } = true;
}


public class SubCommentCollection : HaWeb.XMLParser.IXMLCollection {
    public string Key { get; } = "subcomments";
    public string[] xPath { get; } = new string[] { "/opus/data/kommentare/kommcat/kommentar/subsection", "/opus/kommentare/kommcat/kommentar/subsection" };
    public Func<XElement, string?> GenerateKey { get; } = CommentCollectionHelpers.GetKey;
    public Func<XElement, IDictionary<string, string>?>? GenerateDataFields { get; } = GetDataFields;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, ILookup<string, CollectedItem>>?>? GroupingsGeneration { get; } = CommentCollectionHelpers.GetLookups;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, IEnumerable<CollectedItem>>?>? SortingsGeneration { get; } = null;
    public HaWeb.XMLParser.IXMLCollection[]? SubCollections { get; } = null;
    public bool Searchable { get; } = true;

    public static IDictionary<string, string>? GetDataFields(XElement element) {
        var res = new Dictionary<string, string>();
        var lemma = element.Descendants("lemma");
        if (!lemma.Any() || String.IsNullOrWhiteSpace(lemma.First().Value)) return null;
        res.Add("lemma", lemma.First().Value);
        var comment = element.Ancestors("kommcat").First();
        var type = (string?)comment.Attribute("value");
        if (String.IsNullOrWhiteSpace(type)) return null;
        res.Add("type", type);
        var parent = element.Ancestors("kommentar").First();
        var parentid = (string?)parent.Attribute("id");
        if (String.IsNullOrWhiteSpace(parentid)) return null;
        res.Add("parent", parentid);
        return res;
    }
}