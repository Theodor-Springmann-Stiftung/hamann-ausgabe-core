namespace HaWeb.XMLTests;

using HaWeb.Models;
using HaWeb.XMLParser;

public class XMLTestService : IXMLTestService {
    public Dictionary<string, INodeRule>? Ruleset { get; private set; }
    public Dictionary<string, ICollectionRule>? CollectionRuleset { get; private set; }
    public XMLTestService() {
        var roottypes = _GetAllTypesThatImplementInterface<INodeRule>().ToList();
        roottypes.ForEach( x => {
            if (this.Ruleset == null) this.Ruleset = new();
            var instance = (INodeRule)Activator.CreateInstance(x)!;
            if (instance != null) this.Ruleset.Add(instance.Name, instance);
        });

        var collectionruleset = _GetAllTypesThatImplementInterface<ICollectionRule>().ToList();
        collectionruleset.ForEach( x => {
            if (this.CollectionRuleset == null) this.CollectionRuleset = new();
            var instance = (ICollectionRule)Activator.CreateInstance(x)!;
            if (instance != null) this.CollectionRuleset.Add(instance.Name, instance);
        });
    }

    public Dictionary<string, SyntaxCheckModel>? Test(Dictionary<string, FileList?>? _Loaded, Dictionary<string, SyntaxCheckModel> _Results) {
        if (_Loaded == null) return null;
        var tester = new XMLTester(this, _Loaded, _Results);
        return tester.Test();
    }

    private IEnumerable<Type> _GetAllTypesThatImplementInterface<T>() {
        return System.Reflection.Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
    }
}