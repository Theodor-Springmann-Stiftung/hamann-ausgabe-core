using HaXMLReader.EvArgs;
using System.Linq;

namespace HaDocument {
    static class Helpers {
        internal static string GetElementString(Tag tag) {
            var res = "<" + (tag.EndTag ? "/" : "") + tag.Name.ToLower();
            if (!tag.EndTag && tag.Values != null && tag.Values.Any()) {
                foreach (var kvp in tag.Values) {
                    res += " " + kvp.Key.ToLower() + "=\"" + kvp.Value.ToLower() + "\"";
                }
            }
            return res + (tag.IsEmpty ? "/" : "") + ">";
        }
    }
}