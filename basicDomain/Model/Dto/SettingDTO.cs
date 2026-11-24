#nullable enable

namespace SemBackDotNet10.basicDomain.Model.Dto;

public class SettingDTO
{
    public long? SettingId { get; set; }
    public string? SettingKey { get; set; }
    public string? SettingValue { get; set; }
    public long? TenantId { get; set; }

    public SettingDTO() { }

    public SettingDTO(long? settingId, string? settingKey, string? settingValue, long? tenantId)
    {
        SettingId = settingId;
        SettingKey = settingKey;
        SettingValue = settingValue;
        TenantId = tenantId;
    }
}
