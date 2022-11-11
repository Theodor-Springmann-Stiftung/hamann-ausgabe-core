using System.Web;

namespace HaWeb.HTMLHelpers;
public static class StringHelpers {
    public static string GetEnumerationString(IEnumerable<string> strlist) {
        var res = string.Empty;
        foreach (var str in strlist) {
            if (str != strlist.First())
                if (str == strlist.Last())
                    res += " und " + str;
                else
                    res += ", " + str;
            else
                res += str;
        }
        return res;
    }
}