#nullable enable
using System;

namespace SemBackDotNet10.Common.exception;

// Spring AccessDeniedException has no direct CLR equivalent.
public class AccessForbiddenException : UnauthorizedAccessException
{
    public string[]? Args { get; set; }
    public AccessForbiddenException() : base("کاربر فاقد سطح دسترسی لازم میباشد") { }
    public AccessForbiddenException(string? message, params string[] args) : base(message)
    {
        Args = args;
    }
}
