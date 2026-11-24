namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface IBruteForceProtectionService
{
    bool IsBlocked(string key);

    void RegisterFailure(string key);

    void RegisterSuccess(string key);
}
