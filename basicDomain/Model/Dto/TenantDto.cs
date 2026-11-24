#nullable enable
namespace SemBackDotNet10.basicDomain.Model.Dto;

public class TenantDto
{
    public long? Id { get; set; }
    public string? TenantName { get; set; }
    public string? TenantCode { get; set; }
    public long? TenantParentId { get; set; }

    public TenantDto() { }

    public TenantDto(long? id, string? tenantName, string? tenantCode, long? tenantParentId)
    {
        Id = id;
        TenantName = tenantName;
        TenantCode = tenantCode;
        TenantParentId = tenantParentId;
    }
}
