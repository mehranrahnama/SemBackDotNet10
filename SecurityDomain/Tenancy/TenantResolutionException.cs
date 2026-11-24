namespace SemBackDotNet10.SecurityDomain.Tenancy;

public sealed class TenantResolutionException : Exception
{
    public TenantResolutionException() : base("Tenant could not be resolved.") { }
}
