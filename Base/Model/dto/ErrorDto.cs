using System.Text.Json.Serialization;

namespace SemBackDotNet10.Base.model.dto;

public sealed class ErrorDto
{
    // Renamed in C# because a nested type and property cannot both be 'Status'.
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ErrorStatus { WARNING, ERROR }
    [JsonPropertyName("status")] public ErrorStatus? Status { get; }
    [JsonPropertyName("code")] public string? Code { get; }
    [JsonPropertyName("message")] public string? Message { get; }
    [JsonPropertyName("detail")] public object? Detail { get; }
    private ErrorDto(ErrorStatus? status, string? code, string? message, object? detail)
        => (Status, Code, Message, Detail) = (status, code, message, detail);
    public sealed class Builder
    {
        private ErrorStatus? _status;
        private string? _code, _message;
        private object? _detail;
        public Builder status(ErrorStatus? value) { _status=value; return this; }
        public Builder code(string? value) { _code=value; return this; }
        public Builder message(string? value) { _message=value; return this; }
        public Builder detail(object? value) { _detail=value; return this; }
        public ErrorDto build() => new(_status, _code, _message, _detail);
    }
}
