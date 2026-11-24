using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using SemBackDotNet10.measureDomain.service.interf;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.measureDomain.service.impl;

public sealed class HamsooMeasureService(
    HttpClient client, IConfiguration configuration, MeasureUnitServiceImpl units,
    IMeasureRepositoryService repositories, IMeasureCatalogService catalogs)
{
    public async Task ImportProcessMeasuresAsync(CancellationToken ct = default)
    {
        using var tokenResponse = await client.PostAsJsonAsync(Required("ExternalServices:Hamsoo:LoginUrl"),
            new { username = Required("ExternalServices:Hamsoo:Username"), password = Required("ExternalServices:Hamsoo:Password") }, ct);
        tokenResponse.EnsureSuccessStatusCode();
        using var tokenJson = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync(ct));
        var token = tokenJson.RootElement.GetProperty("token").GetString();
        if (string.IsNullOrWhiteSpace(token))
            throw new JsonException("Hamsoo returned an empty token.");

        using var request = new HttpRequestMessage(HttpMethod.Get, Required("ExternalServices:Hamsoo:MeasureListUrl").TrimEnd('/') + "/1402");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("curOrgan", "40");
        using var response = await client.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        using var data = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
        foreach (var row in data.RootElement.GetProperty("content").EnumerateArray())
        {
            ct.ThrowIfCancellationRequested();
            var process = JsonNode.Parse(row.GetRawText())!.AsObject();
            var unit = await units.FindOrInsertProcessMeasureUnitAsync(row.GetProperty("kpiUnit"), ct);
            var repository = repositories.findOrInsertProcessMeasureRepository(process, unit);
            catalogs.findOrInsertMeasureCatalog(process, repository);
        }
    }

    private string Required(string key) => configuration[key] is { Length: > 0 } value
        ? value : throw new InvalidOperationException($"Missing Hamsoo configuration: {key}.");
}
