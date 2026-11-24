namespace SemBackDotNet10.Common.Model;

public sealed class ErrorViewModel
{
    public string? RequestId { get; init; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
