using System.Text.Json;

namespace SemBackDotNet10.analysisDomain.service.impl.resource;

/// <summary>معادل کمکی org.json.JSONObject جاوا برای خواندن بدنه‌های JSON خام.</summary>
public static class AnalysisJsonHelper
{
    public static JsonElement Parse(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
            throw new InvalidOperationException("JSON body is required");
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.Clone();
    }

    public static bool Has(JsonElement obj, string key) =>
        obj.ValueKind == JsonValueKind.Object
        && obj.TryGetProperty(key, out var v)
        && v.ValueKind != JsonValueKind.Null;

    /// <summary>معادل optString(key, null).</summary>
    public static string? OptString(JsonElement obj, string key)
    {
        if (!Has(obj, key)) return null;
        var v = obj.GetProperty(key);
        return v.ValueKind == JsonValueKind.String ? v.GetString() : v.ToString();
    }

    /// <summary>معادل getString؛ در نبود کلید خطا می‌دهد.</summary>
    public static string GetString(JsonElement obj, string key) =>
        OptString(obj, key) ?? throw new InvalidOperationException($"JSONObject[\"{key}\"] not found.");

    /// <summary>معادل getLong؛ در نبود کلید خطا می‌دهد.</summary>
    public static long GetLong(JsonElement obj, string key) =>
        OptLong(obj, key) ?? throw new InvalidOperationException($"JSONObject[\"{key}\"] not found.");

    public static long? OptLong(JsonElement obj, string key)
    {
        if (!Has(obj, key)) return null;
        var v = obj.GetProperty(key);
        if (v.ValueKind == JsonValueKind.Number)
            return v.TryGetInt64(out var l) ? l : (long)v.GetDouble();
        if (v.ValueKind == JsonValueKind.String && long.TryParse(v.GetString(), out var p))
            return p;
        throw new InvalidOperationException($"JSONObject[\"{key}\"] is not a long.");
    }

    public static int? OptInt(JsonElement obj, string key) => (int?)OptLong(obj, key);

    public static int GetInt(JsonElement obj, string key) => (int)GetLong(obj, key);

    public static bool? OptBool(JsonElement obj, string key)
    {
        if (!Has(obj, key)) return null;
        var v = obj.GetProperty(key);
        return v.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String when bool.TryParse(v.GetString(), out var b) => b,
            _ => throw new InvalidOperationException($"JSONObject[\"{key}\"] is not a boolean.")
        };
    }
}
