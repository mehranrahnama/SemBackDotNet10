using System;

namespace SemBackDotNet10.Base.service.inter;

/// <summary>معادل SearchCriteria.java — @Getter/@Setter به property عمومی؛ سازنده حفظ شده.</summary>
public class SearchCriteria
{
    public SearchCriteria(string key, object? value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; set; }
    public object? Value { get; set; }
}
