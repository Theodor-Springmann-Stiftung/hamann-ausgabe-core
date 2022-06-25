namespace HaWeb.Models;
using HaWeb.XMLParser;

public class ItemsCollection {
    public string Name { get; private set; }
    public Dictionary<string, CollectedItem> Items { get; private set; }
    public bool Searchable { get; private set; }
    public IXMLRoot Root { get; private set; }
    public Func<List<CollectedItem>, Dictionary<string, Lookup<string, CollectedItem>>?>? GroupingsGeneration { get; private set; }
    public Func<List<CollectedItem>, Dictionary<string, List<CollectedItem>>?>? SortingsGeneration { get; private set; }

    public Dictionary<string, Lookup<string, CollectedItem>>? Groupings { get; private set; }
    public Dictionary<string, List<CollectedItem>>? Sortings { get; private set; }

    public ItemsCollection(
        string name,
        bool searchable,
        IXMLRoot root,
        Func<List<CollectedItem>, Dictionary<string, Lookup<string, CollectedItem>>?>? groupingsFunc = null,
        Func<List<CollectedItem>, Dictionary<string, List<CollectedItem>>?>? sortingsFunc = null
    ) {
        this.Name = name;
        this.Searchable = searchable;
        this.Root = root;
        this.GroupingsGeneration = groupingsFunc;
        this.SortingsGeneration = sortingsFunc;
        this.Items = new Dictionary<string, CollectedItem>();
    }

    public void GenerateGroupings(
        Func<List<CollectedItem>, Dictionary<string, Lookup<string, CollectedItem>>?>? groupingsFunc = null
    ) {
        if (groupingsFunc != null)
            this.GroupingsGeneration = groupingsFunc;
        if (this.GroupingsGeneration != null && this.Items.Any())
            this.Groupings = GroupingsGeneration(this.Items.Values.ToList());
    }

    public void GenerateSortings(
        Func<List<CollectedItem>, Dictionary<string, List<CollectedItem>>?>? sortingsFunc = null
    ) {
        if (sortingsFunc != null)
            this.SortingsGeneration = sortingsFunc;
        if (this.SortingsGeneration != null && this.Items.Any()) 
            this.Sortings = SortingsGeneration(this.Items.Values.ToList());
    }
}
