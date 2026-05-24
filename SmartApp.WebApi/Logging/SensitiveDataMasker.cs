using System.Text.Json;
using System.Text.RegularExpressions;

namespace SmartApp.WebApi.Logging;

public static class SensitiveDataMasker
{
    private const string MaskValue = "***MASKED***";

    public static string MaskJson(string json, IEnumerable<string> sensitiveFields)
    {
        if (string.IsNullOrWhiteSpace(json)) return json;

        try
        {
            using var document = JsonDocument.Parse(json);
            var masked = MaskElement(document.RootElement, sensitiveFields
                .Select(f => f.ToLowerInvariant())
                .ToHashSet());

            return JsonSerializer.Serialize(masked, new JsonSerializerOptions
            {
                WriteIndented = false
            });
        }
        catch
        {
            // ← if JSON parsing fails, mask entire body
            return MaskValue;
        }
    }

    private static object MaskElement(JsonElement element, HashSet<string> sensitiveFields)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var dict = new Dictionary<string, object?>();
                foreach (var prop in element.EnumerateObject())
                {
                    dict[prop.Name] = sensitiveFields.Contains(prop.Name.ToLowerInvariant())
                        ? MaskValue
                        : MaskElement(prop.Value, sensitiveFields);
                }
                return dict;

            case JsonValueKind.Array:
                return element.EnumerateArray()
                    .Select(e => MaskElement(e, sensitiveFields))
                    .ToList();

            case JsonValueKind.String: return element.GetString()!;
            case JsonValueKind.Number: return element.GetRawText();
            case JsonValueKind.True: return true;
            case JsonValueKind.False: return false;
            case JsonValueKind.Null: return null!;
            default: return element.GetRawText();
        }
    }
}