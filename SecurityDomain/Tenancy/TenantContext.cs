namespace SemBackDotNet10.SecurityDomain.Tenancy
{
    public sealed class TenantContext
    {
        private long? _tenantId;

        public long TenantId =>
            _tenantId ?? throw new InvalidOperationException(
                "Tenant has not been resolved.");

        public bool HasTenant => _tenantId.HasValue;

        public void SetTenant(long tenantId)
        {
            if (tenantId <= 0)
                throw new ArgumentOutOfRangeException(nameof(tenantId));

            _tenantId = tenantId;
        }
    }
}
