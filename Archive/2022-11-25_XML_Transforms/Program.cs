using System.Xml;
using System.Xml.Linq;

// See https://aka.ms/new-console-template for more information

static (XDocument, XDocument) LoadDocuments(string tradfile, string reffile)
{
    var tradoc = XDocument.Load(tradfile, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
    var refdoc = XDocument.Load(reffile, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
    return (tradoc, refdoc);
}

static void TestOpenTags(XDocument traditions)
{
    Console.WriteLine("Teste, ob es Tags gibt, die vor einem neuem <app>-Tag noch geöffnet sind.");
    var apps = traditions.Descendants("app");
    bool any = false;
    foreach (var app in apps)
    {
        var anc = app.Ancestors();
        if (anc.Count() > 4)
        {
            Console.WriteLine("Gefunden: <app> Zeile " + ((IXmlLineInfo)app).LineNumber);
            foreach (var a in anc)
            {
                Console.WriteLine(a.Name);
            }
            any = true;
        }
    }
    if (any) throw new Exception("Manche Tags sind noch offen zwischen den apps!");
}

static HashSet<string> GetNormalizeNames(XDocument traditions)
{
    var appnames = new HashSet<string>();
    var apps = traditions.Descendants("app");
    foreach (var app in apps)
    {
        if (app.Value.EndsWith(":"))
        {
            app.SetValue(app.Value.Replace(":", ""));
        }
        if (!appnames.Contains(app.Value)) ;
        appnames.Add(app.Value);
    }

    foreach (var name in appnames)
        Console.WriteLine("App-Name gefunden: " + name);
    return appnames;
}

static Dictionary<string, string> GetAppNumbers(XDocument references, HashSet<string> names)
{
    var ret = references.Descendants("appDef").ToDictionary(x => x.Attribute("name").Value, x => x.Attribute("index").Value);
    foreach (var r in ret)
    {
        if (names.Contains(r.Key)) names.Remove(r.Key);
        Console.WriteLine("AppDef gefunden für " + r.Key);
    }

    if (names.Any())
    {
        foreach (var n in names)
            Console.WriteLine("Keine AppDef für " + n + " gefunden.");
        throw new Exception("AppDefs nicht vollständig. Bitte ergänzen.");
    }
    return ret;
}

static List<(XElement, List<XNode>)> GetAppContents(XDocument traditions)
{
    var ret = new List<(XElement, List<XNode>)>();
    var apps = traditions.Descendants("app");
    foreach (var app in apps)
    {
        var sibs = app.NodesAfterSelf();
        var content = new List<XNode>();
        foreach (var s in sibs)
        {
            if (s.NodeType == XmlNodeType.Element && ((XElement)s).Name == "app") break;
            content.Add(s);
        }
        ret.Add((app, content));
    }
    return ret;
}

static void ReplaceStructure(List<(XElement appNode, List<XNode> childNodes)> nodes, Dictionary<string, string> appNumbers)
{
    foreach (var n in nodes)
    {
        var name = n.appNode.Value;
        var number = appNumbers[name];
        n.appNode.SetValue(string.Empty);
        n.appNode.SetAttributeValue("ref", number);
        n.appNode.Add(n.childNodes);

        foreach (var node in n.childNodes)
            node.Remove();
    }
}

static void CleanupLineBreaks(XDocument traditions) {
    var apps = traditions.Descendants("app");
    foreach (var a in apps) {
        var children = a.Nodes();
        RemoveLineBreaks(children);

        children = children.Reverse();
        RemoveLineBreaks(children);

        var siblings = a.NodesAfterSelf();
        RemoveLineBreaks(siblings);

        siblings = a.NodesBeforeSelf();
        RemoveLineBreaks(siblings);
    }
}

static void RemoveLineBreaks(IEnumerable<XNode> nodes) {
    foreach (var c in nodes) {
        if (c.NodeType == XmlNodeType.Element) {
            var e = (XElement)c;
            if (e.Name == "line" && e.Attribute("type") != null && e.Attribute("type").Value == "break")
                c.Remove();
            break;
        }
        if (!String.IsNullOrWhiteSpace(c.ToString()))
            break;
    }
}


static void FindSingles(XDocument traditions) {
    var tradition = traditions.Descendants("letterTradition");
    foreach (var t in tradition) {
        var nodes = t.Nodes();
        var singles = new List<XNode> ();
        var isnonws = false;
        foreach (var n in nodes) {
            if (n.NodeType == XmlNodeType.Element && ((XElement)n).Name == "app")
                break;
            singles.Add(n);
            if (!String.IsNullOrWhiteSpace(n.ToString()))
                isnonws = true;
        }
        if (isnonws) {
            var elem = new XElement("app");
            elem.SetAttributeValue("ref", "-1");
            elem.Add(singles);
            t.AddFirst(elem);
            foreach (var n in singles)
                n.Remove();
        }
    }
}

static void Cleanup(XDocument traditions) {
    var apps = traditions.Descendants("app");
    foreach (var a in apps) {
        if (a.Value.Last() != '\n')
            a.Add("\n");
        a.AddAfterSelf("\n");
    }

    var lt = traditions.Descendants("letterTradition");
    foreach (var l in lt)
        l.AddAfterSelf("\n\n");
}

static void Save((XDocument tradoc, XDocument refdoc) docs)
{
    docs.tradoc.Save("./traditionsnew.xml", SaveOptions.DisableFormatting);
}

var documents = LoadDocuments("./traditions.xml", "./references.xml");
TestOpenTags(documents.Item1);
var names = GetNormalizeNames(documents.Item1);
var appnumbers = GetAppNumbers(documents.Item2, names);
var contents = GetAppContents(documents.Item1);
ReplaceStructure(contents, appnumbers);
FindSingles(documents.Item1);
CleanupLineBreaks(documents.Item1);
Cleanup(documents.Item1);
Save(documents);