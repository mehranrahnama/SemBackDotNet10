namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface IPasswordVerifier
{
    bool Verify(string hashedPassword, string providedPassword);
}
