using System.Xml.Linq;

public static class MarginalsTransform {
    public static List<XElement> Marginals;

    public static void Transform() {
        var lu = Marginals
            .Where(x => x.HasAttributes &&
                x.Attribute("line") != null &&
                x.Attribute("page") != null &&
                x.Attribute("letter") != null)
            .GroupBy(x => 
                x.Attribute("letter")!.Value + 
                "-" + 
                x.Attribute("page")!.Value + 
                "-" + 
                x.Attribute("line")!.Value
        );

        foreach (var l in lu) {
            if (l.Count() > 1) {
                var list = l
                    .Where(x => x.HasAttributes && x.Attribute("index") != null && Int32.TryParse(x.Attribute("index")!.Value, out var _) != false)
                    .OrderBy(y => Int32.Parse(y.Attribute("index")!.Value));
                var i = 1;
                foreach (var e in list) {
                    e.Attribute("index")!.Remove();
                    e.Add(new XAttribute("sort", i.ToString()));
                    i++;
                }
            } else if (l.Count() == 1) {
                if (l.First().HasAttributes && l.First().Attribute("index") != null) {
                    l.First().Attribute("index")!.Remove();
                }
            }
        }
    }
}