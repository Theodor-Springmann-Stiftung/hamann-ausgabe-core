namespace HaWeb.XMLParser;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;


public class IdentificationStringJSONConverter : JsonConverter<(string?, string?)>
{
    public override (string?, string?) Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) {
            var s = reader.GetString();
            if (s == null) return (null, null);
            var split = s.Split('-');
            string? str1 = null;
            if (!String.IsNullOrWhiteSpace(split[0])) str1 = split[0];
            if (s.Length > 1 && !String.IsNullOrWhiteSpace(split[1])) return (str1, split[1]);
            else return (str1, null);
        }

    public override void Write(
        Utf8JsonWriter writer,
        (string?, string?) value,
        JsonSerializerOptions options)
        {
            if (value.Item1 == null && value.Item2 == null) return;
            var res = string.Empty;
            if (value.Item1 != null) res += value.Item1;
            if (value.Item2 != null) res += "-" + value.Item2;
            writer.WriteStringValue(res);
        }
}

