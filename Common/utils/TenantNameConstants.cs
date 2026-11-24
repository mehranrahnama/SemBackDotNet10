// C# migration of utils/TenantNameConstants.java

namespace Common.utils;

public static class TenantNameConstants
{
 public const string TENANT_HOLDING="MSCHOLDING", TENANT_HOSCO="HOSCO", TENANT_IRISA="IRISA", TENANT_SANGAN="SANGAN", TENANT_SEFIDDASH="SEFIDDASHT", TENANT_BASA="BASA";
 public static IReadOnlySet<string> SINGLE_STRATEGYOBJ_TENANTS { get; } = new HashSet<string>{"HOSCO","SEFIDDASHT"};
 public static bool IsSingleStrategyObjTenant(string tenantName) => SINGLE_STRATEGYOBJ_TENANTS.Contains(tenantName);
 public static bool IsParentOnlyTenant(string? tenantName) => string.Equals(TENANT_HOSCO,tenantName,StringComparison.OrdinalIgnoreCase);
}
