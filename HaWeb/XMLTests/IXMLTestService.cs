namespace HaWeb.XMLTests;

public interface IXMLTestService {

    public Dictionary<string, INodeRule>? Ruleset { get; }
    public Dictionary<string, ICollectionRule>? CollectionRuleset { get; }

    public void Test();
}