namespace HaWeb.SearchHelpers;
using System.Text;

public static class StringHelpers {
    public static string NormalizeWhiteSpace(string input, char normalizeTo = ' ', bool toLower = true) {
        if (string.IsNullOrEmpty(input)) {
            return string.Empty;
        }

        StringBuilder output = new StringBuilder();

        // TODO: what about punctuation (char.IsPunctuation()) ? what about spaces?

        // Remove all whitespace, search becomes whitespace insensitive
        // foreach (var c in input) 
        //     if (!char.IsWhiteSpace(c))  {
        //         if (toLower) output.Append(char.ToLower(c));
        //         else output.Append(c);
        //     }

        // Collapse all whitespace into a single whitespace:
        bool skipped = false;

        foreach (char c in input) {
            // TODO: punctuation
            if (char.IsWhiteSpace(c)) {
                if (!skipped) {
                    output.Append(normalizeTo);
                    skipped = true;
                }
            } else {
                skipped = false;
                if (toLower) output.Append(char.ToUpperInvariant(c));
                else output.Append(c);
            }
        }

        return output.ToString();
    }
}