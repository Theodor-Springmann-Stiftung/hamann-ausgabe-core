namespace HaWeb.XMLTests;
using HaWeb.XMLParser;

public class XMLTestService : IXMLTestService {
    public Dictionary<string, INodeRule>? Ruleset { get; private set; }
    public Dictionary<string, ICollectionRule>? CollectionRuleset { get; private set; }
    public XMLTestService() {
        var roottypes = _GetAllTypesThatImplementInterface<INodeRule>().ToList();
        roottypes.ForEach( x => {
            if (this.Ruleset == null) this.Ruleset = new Dictionary<string, INodeRule>();
            var instance = (INodeRule)Activator.CreateInstance(x)!;
            if (instance != null) this.Ruleset.Add(instance.Name, instance);
        });

        var collectionruleset = _GetAllTypesThatImplementInterface<ICollectionRule>().ToList();
        collectionruleset.ForEach( x => {
            if (this.CollectionRuleset == null) this.CollectionRuleset = new Dictionary<string, ICollectionRule>();
            var instance = (ICollectionRule)Activator.CreateInstance(x)!;
            if (instance != null) this.CollectionRuleset.Add(instance.Name, instance);
        });
    }

    public void Test(IXMLInteractionService _XMLService) {
        var docs = _XMLService.GetLoaded();
        if (docs == null) return;
        var tester = new XMLTester(this, docs);
        tester.Test();
    }

    private IEnumerable<Type> _GetAllTypesThatImplementInterface<T>()
    {
        return System.Reflection.Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
    }
}