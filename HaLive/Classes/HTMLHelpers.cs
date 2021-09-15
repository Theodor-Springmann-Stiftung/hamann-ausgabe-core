using System;
using System.Collections.Generic;
using System.Linq;

namespace HaLive
{
    public struct Attribute
    {
        public string Name;
        public string Value;
    }

    public static class HTMLHelpers
    {
        public static string[] MonthNames = { "", "Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember" };

        private static Dictionary<char, int> RomanMap = new Dictionary<char, int>()
        {
                {'I', 1},
                {'V', 5},
                {'X', 10},
                {'L', 50},
                {'C', 100},
                {'D', 500},
                {'M', 1000}
        };

        public static int RomanToInteger(string roman)
        {
            var ro = roman.ToUpper();
            int number = 0;
            for (int i = 0; i < roman.Length; i++)
            {
                if (RomanMap.ContainsKey(ro[i]) && (i + 1 >= ro.Length || RomanMap.ContainsKey(ro[i + 1])))
                {
                    if (i + 1 < ro.Length && RomanMap[ro[i]] < RomanMap[ro[i + 1]])
                    {
                        number -= RomanMap[ro[i]];
                    }
                    else
                    {
                        number += RomanMap[ro[i]];
                    }
                }
                else return 0;
            }
            return number;
        }

        public static int RomanOrNumberToInt(string number)
        {
            var a = 0;
            if (Int32.TryParse(number, out a)) return a;
            else return RomanToInteger(number);
        }
        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) return string.Empty;
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            return string.Empty;
        }

        public static string CreateElement(string elementname, string classes = "", string ids = "")
        {
            string res = "<" + elementname;
            if (!String.IsNullOrWhiteSpace(classes))
                if (elementname == "button")
                    res += CreateAttribute(new HaLive.Attribute() { Name = "type", Value = classes });
                else                    
                    res += CreateAttribute(new HaLive.Attribute() { Name = "class", Value = classes });
            if (!String.IsNullOrWhiteSpace(ids))
                if (elementname == "a")
                    res += CreateAttribute(new HaLive.Attribute() { Name = "href", Value = ids });
                else
                    res += CreateAttribute(new HaLive.Attribute() { Name = "id", Value = ids });
            return res + ">";
        }

        public static string CreateCustomElement(string elementname, params HaLive.Attribute[] attributes)
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

        public static string CreateAttribute(HaLive.Attribute attr)
            => " " + attr.Name + "=\"" + attr.Value + "\" ";

        public static string CreateEmptyElement(string elementname, string classes = "", string ids = "")
        {
            string res = "<" + elementname;
            if (!String.IsNullOrWhiteSpace(classes))
                res += CreateAttribute(new HaLive.Attribute() { Name = "class", Value = classes });
            if (!String.IsNullOrWhiteSpace(ids))
                res += CreateAttribute(new HaLive.Attribute() { Name = "id", Value = ids });
            return res + "></" + elementname + ">";
        }

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
}