namespace HaWeb.HTMLHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

public static class TagHelpers
{
    public struct Attribute
    {
        public string Name;
        public string Value;
    }

    public static string CreateElement(string elementname, string classes = "", string ids = "")
    {
        string res = "<" + elementname;
        if (!String.IsNullOrWhiteSpace(classes))
            if (elementname == "button")
                res += CreateAttribute(new Attribute() { Name = "type", Value = classes });
            else                    
                res += CreateAttribute(new Attribute() { Name = "class", Value = classes });
        if (!String.IsNullOrWhiteSpace(ids))
            if (elementname == "a")
                res += CreateAttribute(new Attribute() { Name = "href", Value = ids });
            else
                res += CreateAttribute(new Attribute() { Name = "id", Value = ids });
        return res + ">";
    }

    public static string CreateCustomElement(string elementname, params Attribute[] attributes)
    {
        string res = "<" + elementname;
        if (!(attributes.Length == 0))
        {
            foreach (var attrib in attributes)
            {
                res += CreateAttribute(attrib);
            }
        }
        return res + ">";
    }


    public static string CreateEndElement(string elementname)
        => "</" + elementname + ">";

    public static string CreateAttribute(Attribute attr)
        => " " + attr.Name + "=\"" + attr.Value + "\" ";

    public static string CreateEmptyElement(string elementname, string classes = "", string ids = "")
    {
        string res = "<" + elementname;
        if (!String.IsNullOrWhiteSpace(classes))
            res += CreateAttribute(new Attribute() { Name = "class", Value = classes });
        if (!String.IsNullOrWhiteSpace(ids))
            res += CreateAttribute(new Attribute() { Name = "id", Value = ids });
        return res + "></" + elementname + ">";
    }
}