#nullable enable
using System;

namespace SemBackDotNet10.Common.exception;

public class ExternalUserNotFoundException : Exception
{
    public ExternalUserNotFoundException() { }
    public ExternalUserNotFoundException(string? message) : base(message) { }
    public ExternalUserNotFoundException(string? message, Exception? cause) : base(message, cause) { }
}
