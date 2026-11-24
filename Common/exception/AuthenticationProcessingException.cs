#nullable enable
using System;

namespace SemBackDotNet10.Common.exception;

public class AuthenticationProcessingException : Exception
{
    public object?[]? Args { get; }
    public AuthenticationProcessingException() { }
    public AuthenticationProcessingException(string? message) : base(message) { }
    public AuthenticationProcessingException(string? message, params object?[] args) : base(message)
    {
        Args = args;
    }
}
