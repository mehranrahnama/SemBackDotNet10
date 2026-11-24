using System.Text.Json;

namespace SemBackDotNet10.analysisDomain.service.impl.porter;

/// <summary>معادل متدهای getString / getLong / isNull از org.json.JSONObject.</summary>
internal static class PorterJsonHelper
{
    public static JsonElement Parse(string? body)
    {
        using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(body) ? "{}" : body);
        return document.RootElement.Clone();
    }

    private static bool IsNull(JsonElement json, string key) =>
        json.ValueKind != JsonValueKind.Object
        || !json.TryGetProperty(key, out var value)
        || value.ValueKind == JsonValueKind.Null
        || value.ValueKind == JsonValueKind.Undefined;

    /// <summary>مقدار رشته‌ای؛ در صورت نبودن کلید null برمی‌گردد (فیلتر اختیاری).</summary>
    public static string? GetString(JsonElement json, string key)
    {
        if (IsNull(json, key))
            return null;
        var value = json.GetProperty(key);
        return value.ValueKind == JsonValueKind.String ? value.GetString() : value.GetRawText();
    }

    public static long? OptLong(JsonElement json, string key)
    {
        if (IsNull(json, key))
            return null;
        var value = json.GetProperty(key);
        return value.ValueKind switch
        {
            JsonValueKind.Number => value.TryGetInt64(out var l) ? l : (long)value.GetDouble(),
            JsonValueKind.String when long.TryParse(value.GetString(), out var parsed) => parsed,
            _ => throw new JsonException($"JSONObject[\"{key}\"] is not a number.")
        };
    }

    /// <summary>معادل getLong: در نبودن کلید خطا می‌دهد.</summary>
    public static long GetLong(JsonElement json, string key) =>
        OptLong(json, key) ?? throw new JsonException($"JSONObject[\"{key}\"] not found.");
}
