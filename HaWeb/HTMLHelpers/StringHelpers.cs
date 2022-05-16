namespace HaWeb.HTMLHelpers;
public static class StringHelpers {
    public static string GetEnumerationString(List<string> strlist)
    {
        var res = "";
        foreach (var str in strlist)
        {   
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