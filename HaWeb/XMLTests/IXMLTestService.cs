namespace HaWeb.XMLTests;

public interface IXMLTestService {

    public Dictionary<string, INodeRule>? Ruleset { get; }

    public void Test();
}