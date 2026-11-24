namespace Common.webServices.ErpService.Interf;
public sealed record ErpSmsResponse(int StatusCode,string? Body);
public interface ErpSetupService
{
    Task<string> GetToken(CancellationToken cancellationToken=default);
    Task<ErpSmsResponse> CallErpSmsService(string message,IReadOnlyList<string> numbers,CancellationToken cancellationToken=default);
}
