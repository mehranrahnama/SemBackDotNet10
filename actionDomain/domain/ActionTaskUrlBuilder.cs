#nullable enable
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SemBackDotNet10.actionDomain.domain;

/// <summary>Task URL construction extracted from the three Java workflows.
/// Expects a valid, already URI-encoded HTTP(S) configuration URL.</summary>
public sealed class ActionTaskUrlBuilder
{
    private readonly string settingTaskUrl;

    public ActionTaskUrlBuilder(string settingTaskUrl)
    {
        ArgumentNullException.ThrowIfNull(settingTaskUrl);
        this.settingTaskUrl = settingTaskUrl;
    }

    public string generateProgressTaskUrl(long? id, long? newStatus, long? oldStatus)
        => Build("Progress/ActionProgress/ActionProgressCartable", id, newStatus, oldStatus);

    public string generateTaskUrl(long? id, long? newStatus, long? oldStatus)
        => Build("Action/ActionManagement/ActionManagementCartable", id, newStatus, oldStatus);

    public string generateReplanTaskUrl(long? id, long? newStatus, long? oldStatus)
        => Build("Action/ActionReplan/ActionReplanCartable", id, newStatus, oldStatus);

    internal string Build(string path, long? id, long? newStatus, long? oldStatus)
    {
        if (!Uri.TryCreate(settingTaskUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            || string.IsNullOrEmpty(uri.Host)
            || Regex.IsMatch(settingTaskUrl, @"\s"))
            throw new ArgumentException("taskUrl must be an already URI-encoded absolute HTTP(S) URL.", nameof(settingTaskUrl));

        // Work on the original string: UriBuilder can canonicalize escaping,
        // default ports and query strings that should remain untouched here.
        var match = Regex.Match(settingTaskUrl,
            @"\A(?<origin>https?://[^/?#]+)(?<path>[^?#]*)(?:\?(?<query>[^#]*))?(?<fragment>\#.*)?\z",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!match.Success)
            throw new ArgumentException("Unsupported taskUrl format.", nameof(settingTaskUrl));

        // Spring path(String) appends rather than resolving a relative URI:
        // '/base' + 'Action/...' is '/baseAction/...', not '/base/Action/...'.
        var combinedPath = Regex.Replace(match.Groups["path"].Value + path, "/{2,}", "/");
        if (!combinedPath.StartsWith("/", StringComparison.Ordinal)) combinedPath = "/" + combinedPath;
        var query = match.Groups["query"].Value;
        if (query.Length > 0 && !query.EndsWith("&", StringComparison.Ordinal)) query += "&";
        query += Parameter("entityId", id)
            + "&" + Parameter("oldStatus", oldStatus)
            + "&" + Parameter("newStatus", newStatus);
        return match.Groups["origin"].Value + combinedPath + "?" + query
            + match.Groups["fragment"].Value;
    }

    // Spring queryParam(name, null) emits a bare parameter, not '=null'.
    private static string Parameter(string name, long? value)
        => value.HasValue ? name + "=" + value.Value.ToString(CultureInfo.InvariantCulture) : name;
}
