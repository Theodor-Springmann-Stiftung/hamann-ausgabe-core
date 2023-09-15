using System.Text;
using System.Text.RegularExpressions;

public static class CharacterEntityReferences {
    public static void Replace(IEnumerable<string> files, IEnumerable<string?> codepoints) {
        foreach (var f in files) {
            Console.WriteLine("Replacing file " + f);
            var t = File.ReadAllText(f);
            foreach (var s in codepoints) {
                if (s != null) {
                    t = t.Replace(s, ConvertStringToCodepoint(s));
                }
            }
            File.WriteAllText(f, t);
        }
    }

    static string ConvertStringToCodepoint(string input) {
        var sb = new StringBuilder();
        for (int i = 0; i < input.Length; i += char.IsSurrogatePair(input, i) ? 2 : 1) {
            int codepoint = char.ConvertToUtf32(input, i);
            sb.Append("&#x").Append($"{codepoint:X}").Append(';');
        }
        return sb.ToString();
    }

    public static HashSet<string?> GetCodePoints(string[] files) {
        HashSet<string?> res = new();
        Regex cphex = new Regex(@"&#x([0-9a-fA-F]{1,4});");
        Regex cpint = new Regex(@"&#([0-9]{1,4});");
        HashSet<string> XMLForbidden = new HashSet<string>(){
            "<",
            "&",
            ">",
            "'",
            "\""
        };

        foreach (var f in files) {
            var t = File.ReadAllText(f);
            
            var ms = cphex.Matches(t);
            foreach (var mat in ms) {
                int value = Convert.ToInt32(String.Join(string.Empty, mat.ToString().Skip(3).SkipLast(1)), 16);
                var c = char.ConvertFromUtf32(value);
                if (!res.Contains(c) && !XMLForbidden.Contains(c)) { 
                    res.Add(c);
                }
            }

            ms = cpint.Matches(t);
            foreach (var mat in ms) {
                int value = Convert.ToInt32(String.Join(string.Empty, mat.ToString().Skip(2).SkipLast(1)));
                var c = char.ConvertFromUtf32(value);
                Console.WriteLine(mat.ToString() + " " + c);
                if (!res.Contains(c) && !XMLForbidden.Contains(c)) { 
                    res.Add(c);
                }
            }
        }
        return res;
}
}