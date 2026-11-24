using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace SemBackDotNet10.dashboardDomain.repository;

/// <summary>معادل Tuple جاوا: ستون‌ها را بدون حساسیت به حروف و بدون «_» نگه می‌دارد.</summary>
public sealed class DbRow
{
    private readonly Dictionary<string, object?> _values = new(StringComparer.OrdinalIgnoreCase);

    public void Set(string column, object? value) => _values[Normalize(column)] = value;

    public object? FirstValue() => _values.Values.FirstOrDefault();
    public bool Has(string column) => _values.ContainsKey(Normalize(column));

    public object? Get(string column) => _values.GetValueOrDefault(Normalize(column));

    public static string Normalize(string name) => name.Replace("_", string.Empty);
}

public static class DbRowMapper
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> Cache = new();

    public static async Task<List<DbRow>> ReadRowsAsync(DbDataReader reader, CancellationToken ct)
    {
        var rows = new List<DbRow>();
        while (await reader.ReadAsync(ct))
        {
            var row = new DbRow();
            for (var i = 0; i < reader.FieldCount; i++)
                row.Set(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
            rows.Add(row);
        }
        return rows;
    }

    public static T Map<T>(DbRow row) where T : new()
    {
        var item = new T();
        foreach (var prop in Cache.GetOrAdd(typeof(T), t => t.GetProperties().Where(p => p.CanWrite).ToArray()))
        {
            if (!row.Has(prop.Name)) continue;
            var value = row.Get(prop.Name);
            if (value is null) continue;
            var target = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            prop.SetValue(item, target == typeof(string)
                ? Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)
                : Convert.ChangeType(value, target, System.Globalization.CultureInfo.InvariantCulture));
        }
        return item;
    }

    public static List<T> MapAll<T>(IEnumerable<DbRow> rows) where T : new() => rows.Select(Map<T>).ToList();
}
