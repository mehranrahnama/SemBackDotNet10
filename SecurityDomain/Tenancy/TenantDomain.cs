namespace SemBackDotNet10.SecurityDomain.Tenancy
{

    public sealed class TenantDomain
    {
        public long Id { get; set; }

        public long TenantId { get; set; }

        public string Domain { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
