namespace HaWeb.Settings.XMLCollections;
using HaWeb.Models;
using System.Xml.Linq;

public class MarginalCollection : HaWeb.XMLParser.IXMLCollection {
    public string Key { get; } = "marginals";
    public string[] xPath { get; } = new string[] { "/opus/data/marginalien/marginal", "/opus/marginalien/marginal" };
    public Func<XElement, string?> GenerateKey { get; } = GetKey;
    public Func<XElement, IDictionary<string, string>?>? GenerateDataFields { get; } = GetDataFields;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, ILookup<string, CollectedItem>>?>? GroupingsGeneration { get; } = GetLookups;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, IEnumerable<CollectedItem>>?>? SortingsGeneration { get; } = null;
    public HaWeb.XMLParser.IXMLCollection[]? SubCollections { get; } = null;
    public bool Searchable { get; } = true;

    public static Func<XElement, string?> GetKey { get; } = (elem) => {
        var letter = (string?)elem.Attribute("letter");
        var page = (string?)elem.Attribute("page");
        var line = (string?)elem.Attribute("line");
        var sort = (string?)elem.Attribute("sort");
        if (letter == null || page == null || line == null) return null;
        var index = letter + "-" + page + "-" + line + sort ?? "";
        if (index != null && !String.IsNullOrWhiteSpace(index))
            return index;
        else return null;
    };

    public static IDictionary<string, string>? GetDataFields(XElement element) {
        var res = new Dictionary<string, string>();
        var letter = (string?)element.Attribute("letter");
        var page = (string?)element.Attribute("page");
        var line = (string?)element.Attribute("line");
        var sort = (string?)element.Attribute("sort");
        if (letter == null || page == null || line == null) return null;
        res.Add("letter", letter);
        res.Add("page", page);
        res.Add("line", line);
        if (sort != null) res.Add("sort", sort);
        return res;
    }

    public static IDictionary<string, ILookup<string, CollectedItem>>? GetLookups(IEnumerable<CollectedItem> items) {
        var res = new Dictionary<string, ILookup<string, CollectedItem>>();
        var letters = items.Where(x => x["letter"] != null && x["letter"]!.Count() > 0);
        if (letters == null || !letters.Any()) return null;
        res.Add("letter", letters.ToLookup(x => x["letter"]!));
        return res;
    }
}