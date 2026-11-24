namespace SemBackDotNet10.SecurityDomain.Authentication
{

    public sealed record AuthenticationRequest(
        string Username,
        string Password,
        long TenantId);
}
