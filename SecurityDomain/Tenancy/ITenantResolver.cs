namespace SemBackDotNet10.SecurityDomain.Tenancy
{
    public interface ITenantResolver
    {
        Task<long> ResolveTenantIdAsync(
            string host,
            CancellationToken cancellationToken = default);
    }
}
