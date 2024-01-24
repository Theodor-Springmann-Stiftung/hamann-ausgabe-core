namespace HaWeb.HTMLHelpers;

public static class ConversionHelpers {
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

    public static int RomanToInteger(string roman) {
        var ro = roman.ToUpper();
        int number = 0;
        for (int i = 0; i < roman.Length; i++) {
            if (RomanMap.ContainsKey(ro[i]) && (i + 1 >= ro.Length || RomanMap.ContainsKey(ro[i + 1]))) {
                if (i + 1 < ro.Length && RomanMap[ro[i]] < RomanMap[ro[i + 1]]) {
                    number -= RomanMap[ro[i]];
                } else {
                    number += RomanMap[ro[i]];
                }
            } else return 0;
        }
        return number;
    }

    public static int RomanOrNumberToInt(string number) {
        var a = 0;
        if (Int32.TryParse(number, out a)) return a;
        else return RomanToInteger(number);
    }

    public static string ToRomanSafe(string number) {
        var a = 0;
        if (Int32.TryParse(number, out a)) return ToRoman(a);
        else return number;
    }

    public static string ToRoman(int number) {
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
}