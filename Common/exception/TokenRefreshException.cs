#nullable enable
using System;

namespace SemBackDotNet10.Common.exception.handler;

public class TokenRefreshException : Exception
{
    public string[]? Args { get; set; }
    public TokenRefreshException() { }
    public TokenRefreshException(string? message, params string[] args) : base(message)
    {
        Args = args;
    }
}
