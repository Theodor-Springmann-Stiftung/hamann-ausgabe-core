namespace HaWeb.Models;
using HaWeb.XMLParser;

public class ItemsCollection {
    public string Name { get; private set; }
    public IDictionary<string, CollectedItem> Items { get; private set; }
    public IXMLCollection Collection { get; private set; }

    public IDictionary<string, ILookup<string, CollectedItem>>? Groupings { get; private set; }
    public IDictionary<string, IEnumerable<CollectedItem>>? Sortings { get; private set; }

    public ItemsCollection(
        string name,
        IXMLCollection collection
    ) {
        this.Name = name;
        this.Collection = collection;
        this.Items = new Dictionary<string, CollectedItem>();
    }

    public void GenerateGroupings(
        Func<IEnumerable<CollectedItem>, Dictionary<string, ILookup<string, CollectedItem>>?>? groupingsFunc = null
    ) {
        if (groupingsFunc != null) {
            this.Groupings = groupingsFunc(this.Items.Values.ToList());
            return;
        }
        if (Collection.GroupingsGeneration != null && this.Items.Any())
            this.Groupings = Collection.GroupingsGeneration(this.Items.Values.ToList());
    }

    public void GenerateSortings(
        Func<IEnumerable<CollectedItem>, Dictionary<string, IEnumerable<CollectedItem>>?>? sortingsFunc = null
    ) {
        if (sortingsFunc != null) {
            this.Sortings = sortingsFunc(this.Items.Values.ToList());
            return;
        }
        if (Collection.SortingsGeneration != null && this.Items.Any())
            this.Sortings = Collection.SortingsGeneration(this.Items.Values.ToList());
    }

    public CollectedItem? this[string v] {
        get {
            if (Items != null && Items.ContainsKey(v))
                return Items[v];
            return null;
        }
    }
}
