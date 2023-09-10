using HaWeb.Models;
using HaWeb.XMLParser;

namespace HaWeb.XMLTests;

public interface IXMLTestService {

    public Dictionary<string, INodeRule>? Ruleset { get; }
    public Dictionary<string, ICollectionRule>? CollectionRuleset { get; }

    public Dictionary<string, SyntaxCheckModel>? Test(Dictionary<string, FileList?>? _LoadedFiles, Dictionary<string, SyntaxCheckModel> _Results);
}