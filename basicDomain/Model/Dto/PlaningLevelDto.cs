#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SemBackDotNet10.basicDomain.Model.Dto;

/// <summary>معادل PlaningLevelDto جاوا + فیلد Children برای ساخت درخت.</summary>
public class PlaningLevelDto
{
    public long Id { get; set; }

    public string? Code { get; set; }

    public string? DesLevel { get; set; }

    public string? DesThreeChar { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public short TypeLevel { get; set; }

    public string? CreateDate { get; set; }

    public int FactoryId { get; set; } = 1;

    [JsonConverter(typeof(EmptyStringNullableLongConverter))]
    public long? ParentId { get; set; }
    public string? ParentDesLevel { get; set; }
    public short? ParentTypeLevel { get; set; }

    public bool? Operational { get; set; }

    [JsonConverter(typeof(EmptyStringNullableLongConverter))]
    public long? ResponsibleId { get; set; }

    [JsonConverter(typeof(EmptyStringNullableLongConverter))]
    public long? ModirId { get; set; }
    public string? ModirNameF { get; set; }
    public string? ModirFamilyF { get; set; }

    [JsonConverter(typeof(EmptyStringNullableLongConverter))]
    public long? TashilgarId { get; set; }
    public string? TashilgarNameF { get; set; }
    public string? TashilgarFamilyF { get; set; }

    public string? ResponsibleNameF { get; set; }

    public string? ResponsibleFamilyF { get; set; }

    [JsonConverter(typeof(EmptyStringNullableLongConverter))]
    public long? FacilitatorId { get; set; }

    public string? FacilitatorNameF { get; set; }

    public string? FacilitatorFamilyF { get; set; }

    // --- فیلدهای درخت ---
    public bool IsChecked { get; set; }

    public bool IsRoot { get; set; }

    public bool HasChildren { get; set; }

    public List<PlaningLevelDto> Children { get; set; } = new();
}
