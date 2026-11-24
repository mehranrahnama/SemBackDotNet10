using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SemBackDotNet10.service.impl;

public sealed record PoopakSendMessageDto(string? Message, IReadOnlyCollection<string>? Recipients = null);
public sealed record ErpSendMessageDto(string Message, IReadOnlyCollection<string> PhoneNumbers);
public interface IErpSetupService { Task CallErpSmsServiceAsync(string message, IReadOnlyCollection<string> phoneNumbers, CancellationToken ct = default); }

/// <summary>Poopak REST and ERP SMS adapter. Injects HttpClient, ERP SMS service, endpoints and access tokens.</summary>
public sealed class PoopakServiceImpl(HttpClient httpClient, IErpSetupService erpSetupService, string hoscoBasePath, string irisaBasePath, string hoscoToken, string irisaToken, IServiceLogger? logger = null)
{
    private const string SendMessagePath = "/_matrix/client/r0/service/send_message";
    private readonly IServiceLogger _logger = logger ?? new NullServiceLogger();
    public Task<HttpResponseMessage> SendMessageToHOSCOPoopakAsync(PoopakSendMessageDto dto, CancellationToken ct = default)
        => SendMessageAsync(hoscoBasePath, hoscoToken, dto, ct);
    public Task<HttpResponseMessage> SendMessageToIRISAPoopakAsync(PoopakSendMessageDto dto, CancellationToken ct = default)
        => SendMessageAsync(irisaBasePath, irisaToken, dto, ct);
    private async Task<HttpResponseMessage> SendMessageAsync(string basePath, string token, PoopakSendMessageDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (!Uri.TryCreate(basePath.TrimEnd('/') + SendMessagePath, UriKind.Absolute, out var uri)) throw new ArgumentException("Poopak base path must be an absolute URI.", nameof(basePath));
        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim());
        request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        _logger.Info($"Sending Poopak message to {uri.Host}");
        return await httpClient.SendAsync(request, ct);
    }
    public async Task<HttpResponseMessage> SendSMSMessageAsync(ErpSendMessageDto message, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        await erpSetupService.CallErpSmsServiceAsync(message.Message + "\n" + "لغو 11", message.PhoneNumbers, ct);
        return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
    }
}
