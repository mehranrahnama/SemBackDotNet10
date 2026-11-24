#nullable enable
using System;

namespace SemBackDotNet10.Common.exception;

public class CustomException : Exception
{
    public CustomException() { }
    public CustomException(string? message) : base(message) { }
    public CustomException(string? message, Exception innerException) : base(message, innerException) { }
}
