using SemBackDotNet10.SecurityDomain.Interfaces;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;

public sealed class BCryptPasswordVerifier : IPasswordVerifier
{
    public bool Verify(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrEmpty(providedPassword))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
