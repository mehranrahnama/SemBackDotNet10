#nullable enable
using System;

namespace SemBackDotNet10.Common.exception;

public class BusinessException : Exception
{
    // Java's payload is named data. Exception.Data in .NET is an IDictionary.
    // Hide it explicitly so existing business-payload access keeps its meaning.
    public new object? Data { get; }
    public string[]? Args { get; set; }

    public BusinessException() { }

    public BusinessException(string? message, params string[] args) : base(message)
    {
        Args = args;
    }

    public BusinessException(string? message, object? data) : base(message)
    {
        Data = data;
    }
}
