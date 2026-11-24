namespace SemBackDotNet10.SecurityDomain.Configuration;

/// <summary>معادل PasswordEncoder جاوا.</summary>
public interface IPasswordEncoder
{
    string Encode(string rawPassword);

    bool Matches(string rawPassword, string encodedPassword);
}

/// <summary>
/// معادل BCryptPasswordEncoder جاوا.
/// پکیج لازم: dotnet add package BCrypt.Net-Next
/// </summary>
public class BCryptPasswordEncoder : IPasswordEncoder
{
    public string Encode(string rawPassword) =>
        BCrypt.Net.BCrypt.HashPassword(rawPassword, workFactor: 10);

    public bool Matches(string rawPassword, string encodedPassword) =>
        BCrypt.Net.BCrypt.Verify(rawPassword, encodedPassword);
}
