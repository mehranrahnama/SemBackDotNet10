using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using SemBackDotNet10.actionDomain.controller.dto;
using SemBackDotNet10.actionDomain.service.interf;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public class CallActionRestApiServiceImpl(HttpClient client, IConfiguration configuration) : ICallActionRestApiService
{
    private string Setting(string name) => configuration["ExternalServices:CorrectiveAction:" + name]
        is { Length: > 0 } value ? value : throw new InvalidOperationException($"Missing CorrectiveAction setting: {name}");

    public string? getToken(string? userName, string? password)
    {
        using var response = client.PostAsJsonAsync(Setting("GetTokenUrl"), new { username = userName, password })
            .GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        if (response.StatusCode != HttpStatusCode.Created) return null;
        return JsonNode.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult())?["accessToken"]?.ToString();
    }

    public JsonObject? startActionProcess(StartActionDTO body)
    {
        var token = getToken(body.UserDTO.UserName, body.UserDTO.Password);
        if (string.IsNullOrEmpty(token)) throw new BusinessException("دریافت توکن با خطا مواجه شده است");
        using var request = new HttpRequestMessage(HttpMethod.Post, Setting("StartProcessUrl"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(body.ActionDTO);
        using var response = client.SendAsync(request).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        return response.StatusCode == HttpStatusCode.Created
            ? JsonNode.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult()) as JsonObject : null;
    }

    public JsonObject? getRoyalVisionCorrectionAct(long? id)
    {
        if (id is null) throw new ArgumentNullException(nameof(id));
        var token = getToken(Setting("Username"), Setting("Password"));
        if (string.IsNullOrEmpty(token)) throw new BusinessException("دریافت توکن با خطا مواجه شده است");
        var url = QueryHelpers.AddQueryString(Setting("RoyalVisionCorrectionActUrl"), "id", id.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = client.SendAsync(request).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        if (response.StatusCode != HttpStatusCode.OK) return null;
        var result = JsonNode.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        return result switch
        {
            JsonArray { Count: 1 } array when array[0] is JsonObject item => item,
            JsonArray { Count: 0 } => new JsonObject(),
            JsonObject obj => obj,
            _ => throw new JsonException("Unexpected Royal Vision response.")
        };
    }
}
