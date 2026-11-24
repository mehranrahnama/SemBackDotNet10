#nullable enable
using System.Collections.Generic;

namespace SemBackDotNet10.Common.exception;

public class ErrorResponse
{
    public string? Message { get; set; }
    public List<string>? Details { get; set; }

    public ErrorResponse() { }
    public ErrorResponse(string? message)
    {
        Message = message;
        Details = new List<string>();
    }
    public ErrorResponse(string? message, List<string>? details)
    {
        Message = message;
        Details = details;
    }
}
