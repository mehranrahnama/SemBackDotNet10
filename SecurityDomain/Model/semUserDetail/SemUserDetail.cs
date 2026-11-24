using System.Security.Claims;
using UserDto = SemBackDotNet10.basicDomain.Model.Dto.UserDto;
using TenantDto = SemBackDotNet10.basicDomain.Model.Dto.TenantDto;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Model.Dto;

namespace SemBackDotNet10.SecurityDomain.Model.SemUserDetail;

/// <summary>
/// ASP.NET Core equivalent of the Java OAuth2User/UserDetails/OidcUser aggregate.
/// </summary>
public sealed class SemUserDetail
{
    private readonly IRoleRepository _roleRepository;

    public SemUserDetail(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    }

    public SemUserDetail(
        UserDto? user,
        UserAccountDto? account,
        TenantDto? currentOrgan,
        IRoleRepository roleRepository,
        IDictionary<string, object?>? attributes = null)
        : this(roleRepository)
    {
        User = user;
        Account = account;
        CurrentOrgan = currentOrgan;
        Attributes = attributes is null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(attributes);
    }

    public UserDto? User { get; }
    public UserAccountDto? Account { get; }
    public TenantDto? CurrentOrgan { get; private set; }
    public IDictionary<string, object?> Attributes { get; } = new Dictionary<string, object?>();
    public IDictionary<string, object?> Claims => Attributes;

    public string Name =>
        User is null
            ? Account?.AccountName ?? string.Empty
            : string.Join(" ", new[] { User.NameF, User.FamilyF }
                .Where(x => !string.IsNullOrWhiteSpace(x)));

    public string Username => Account?.AccountName ?? User?.UserName ?? string.Empty;
    public string Password => Account?.Password ?? string.Empty;
    public bool IsEnabled => User?.LoginDisabled != true && User?.Retired != true;

    public void SetCurrentOrgan(TenantDto? tenant) => CurrentOrgan = tenant;

    public async Task<IReadOnlyList<(long Id, string Name)>> GetRolesAsync(
        CancellationToken cancellationToken = default)
    {
        if (User?.Id is not long userId || userId <= 0)
            return Array.Empty<(long Id, string Name)>();

        return await _roleRepository.GetForUserAsync(
            User.TenantId ?? CurrentOrgan?.Id ?? 0,
            userId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(
        CancellationToken cancellationToken = default) =>
        (await GetRolesAsync(cancellationToken))
        .Select(role => role.Name)
        .Where(name => !string.IsNullOrWhiteSpace(name))
        .ToArray();

    public Task<IReadOnlyList<string>> GetRoleAsync(
        CancellationToken cancellationToken = default) => GetRoleNamesAsync(cancellationToken);

    public async Task<IReadOnlyList<long>> GetRoleIdAsync(
        CancellationToken cancellationToken = default) =>
        (await GetRolesAsync(cancellationToken))
        .Select(role => role.Id)
        .ToArray();

    public async Task<ClaimsPrincipal> ToClaimsPrincipalAsync(
        CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, Name),
            new(ClaimTypes.NameIdentifier, User?.Id.ToString() ?? string.Empty)
        };

        foreach (var role in await GetRoleNamesAsync(cancellationToken))
            claims.Add(new Claim(ClaimTypes.Role, role));

        foreach (var attribute in Attributes)
        {
            if (attribute.Value is not null)
                claims.Add(new Claim(attribute.Key, Convert.ToString(attribute.Value)!));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "SemUserDetail"));
    }
}
