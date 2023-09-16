namespace HaWeb.Settings.XMLCollections;
using HaWeb.Models;
using System.Xml.Linq;

public class MetaCollection : HaWeb.XMLParser.IXMLCollection {
    public string Key { get; } = "metas";
    public string[] xPath { get; } = new string[] { "/opus/descriptions/letterDesc", "/opus/data/descriptions/letterDesc" };
    public Func<XElement, string?> GenerateKey { get; } = GetKey;
    public Func<XElement, IDictionary<string, string>?>? GenerateDataFields { get; } = null;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, ILookup<string, CollectedItem>>?>? GroupingsGeneration { get; } = null;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, IEnumerable<CollectedItem>>?>? SortingsGeneration { get; } = null;
    public HaWeb.XMLParser.IXMLCollection[]? SubCollections { get; } = null;
    public bool Searchable { get; } = false;

    public static Func<XElement, string?> GetKey { get; } = (elem) => {
        var index = elem.Attribute("letter");
        if (index != null && !String.IsNullOrWhiteSpace(index.Value))
            return index.Value;
        return null;
    };

    public static IDictionary<string, string>? GetDataFields(XElement element) {
        var res = new Dictionary<string, string>();
        var sort = element.Descendants("sort");
        if (sort == null || !sort.Any()) return null;
        var date = (string?)sort.First().Attribute("value");
        var order = (string?)sort.First().Attribute("order");
        if (String.IsNullOrWhiteSpace(date) || !DateTime.TryParse(date, out var dt)) return null;
        res.Add("day", dt.Day.ToString());
        res.Add("month", dt.Month.ToString());
        res.Add("year", dt.Year.ToString());
        if (!String.IsNullOrWhiteSpace(order)) res.Add("order", order);
        else res.Add("order", "0");
        return res;
    }

    public static IDictionary<string, ILookup<string, CollectedItem>>? GetLookups(IEnumerable<CollectedItem> items) {
        var res = new Dictionary<string, ILookup<string, CollectedItem>>();
        var years = items.Where(x => x["year"] != null);
        if (years == null || !years.Any()) return null;
        res.Add("year", years.ToLookup(x => x["year"])!);
        return res;
    }
}