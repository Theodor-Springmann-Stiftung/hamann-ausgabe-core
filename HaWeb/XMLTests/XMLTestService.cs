namespace HaWeb.XMLTests;
using HaWeb.XMLParser;

public class XMLTestService : IXMLTestService {
    private IXMLService _XMLService;
    public Dictionary<string, INodeRule>? Ruleset { get; private set; }
    public Dictionary<string, ICollectionRule>? CollectionRuleset { get; private set; }
    public XMLTestService(IXMLService xmlService) {
        _XMLService = xmlService;

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

    public void Test() {
        var docs = _XMLService.GetUsedDictionary();
        if (docs == null) return;
        foreach (var d in docs.Values) {
            var fl = d.GetFileList();
            if (fl == null) continue;
            foreach (var v in fl) {
                v.ResetLog();
            }
        }
        var tester = new XMLTester(this, _XMLService.GetUsedDictionary());
        tester.Test();
    }

    private IEnumerable<Type> _GetAllTypesThatImplementInterface<T>()
    {
        return System.Reflection.Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
    }
}