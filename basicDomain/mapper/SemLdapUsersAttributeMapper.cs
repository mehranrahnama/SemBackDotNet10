#nullable enable

using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text.RegularExpressions;


namespace SemBackDotNet10.baseDomain.mapper;

public interface ISemLdapUsersAttributeMapper
{
    User MapFromAttributes(IReadOnlyDictionary<string, IReadOnlyList<string>> attributes);
}

public sealed class SemLdapUsersAttributeMapper : ISemLdapUsersAttributeMapper
{
    public User MapFromAttributes(IReadOnlyDictionary<string, IReadOnlyList<string>> attributes)
    {
        ArgumentNullException.ThrowIfNull(attributes);

        var user = new User();

        var firstName = GetFirstString(attributes, "givenName");
        if (firstName is not null)
            user.NameF = firstName;

        var lastName = GetFirstString(attributes, "sn");
        if (lastName is not null)
            user.FamilyF = lastName;

        var userName = GetFirstString(attributes, "sAMAccountName");
        if (userName is not null)
        {
            if (!Regex.IsMatch(userName, @"\p{L}"))
            {
                // Allow only an optional sign and digits, like Java Long.parseLong.
                // Invalid input or overflow intentionally throws an exception.
                user.PersonnelNumber = checked((int)long.Parse(
                    userName,
                    NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture));
            }

            user.UserName = userName;
        }

        var mobile = GetFirstString(attributes, "mobile");
        if (mobile is not null)
            user.CellPhoneNo = mobile;

        return user;
    }

    private static string? GetFirstString(
        IReadOnlyDictionary<string, IReadOnlyList<string>> attributes,
        string attributeName)
    {
        if (!attributes.TryGetValue(attributeName, out var values))
        {
            foreach (var attribute in attributes)
            {
                if (string.Equals(attribute.Key, attributeName, StringComparison.OrdinalIgnoreCase))
                {
                    values = attribute.Value;
                    break;
                }
            }
        }

        if (values is null)
            return null;

        if (values.Count == 0)
        {
            throw new InvalidOperationException(
                $"LDAP attribute '{attributeName}' has no values.");
        }

        return values[0];
    }
}
