// Converted from: camundaDomain/utils/QueryUtils.java
// java.lang.reflect.Field -> reflection روی property های دات‌نت
// نام property ها برای سازگاری با پارامترهای REST کموندا به camelCase تبدیل می‌شود.

using System.Text;

namespace SemBackDotNet10.camundaDomain.utils;

public static class QueryUtils
{
    public static string BuildQueryParamFromClassFields(object obj)
    {
        var builder = new StringBuilder();
        var objType = obj.GetType();
        foreach (var property in objType.GetProperties())
        {
            try
            {
                var fieldName = property.Name;
                if (fieldName.Length > 0)
                    fieldName = char.ToLowerInvariant(fieldName[0]) + fieldName[1..];
                var value = property.GetValue(obj);
                if (value != null)
                {
                    builder.Append(fieldName).Append('=').Append(value).Append('&');
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("failed to build query param from class fields", e);
            }
        }
        if (builder.Length > 0)
        {
            builder.Remove(builder.Length - 1, 1);
        }
        return builder.ToString();
    }
}
