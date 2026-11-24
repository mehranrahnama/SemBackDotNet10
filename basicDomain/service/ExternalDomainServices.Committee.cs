#nullable enable
using System.Globalization;
using System.Text.Json.Nodes;

namespace SemBackDotNet10.service.impl;

internal sealed partial class ExternalDomainServices
{
    public async Task<IReadOnlyList<CommitteeImport>> GetCommitteeTahavolAsync(CancellationToken ct)
    {
        var client = services.GetRequiredService<global::Common.webServices.CommitteeTahavolService>();
        var data = await client.GetCommitteeTahavol(ct);
        var result = new List<CommitteeImport>(data.Count);
        foreach (var item in data)
        {
            var obj = item as JsonObject ?? throw new InvalidOperationException("Invalid committee entry.");
            var members = (obj["member"] as JsonArray ?? throw new InvalidOperationException("Committee member list is missing."))
                .Select(node => new CommitteeMember(
                    int.Parse(RequiredText(node, "role"), CultureInfo.InvariantCulture),
                    long.Parse(RequiredText(node, "personalCode"), CultureInfo.InvariantCulture))).ToList();
            var units = (obj["units"] as JsonArray ?? throw new InvalidOperationException("Committee unit list is missing."))
                .Select(node => new CommitteeUnit(RequiredText(node, "code"), RequiredText(node, "name"))).ToList();
            result.Add(new CommitteeImport(RequiredText(obj, "code"), RequiredText(obj, "name"), members, units));
        }
        return result;
    }

    private static string RequiredText(JsonNode? node, string name) =>
        node is JsonObject obj && obj[name] is { } value && !string.IsNullOrWhiteSpace(value.ToString())
            ? value.ToString() : throw new InvalidOperationException($"Committee field '{name}' is missing.");
}
