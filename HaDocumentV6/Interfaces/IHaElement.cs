using System.Xml.Linq;

public interface IHaElement : IComparable {
    abstract public string ElementName { get; }
    abstract public string[] XPath { get; }
    abstract public string ElementRules { get; }
    abstract public XElement? XElement { get; }
    abstract public bool Searchable { get; }
    abstract public string GetKey();
}