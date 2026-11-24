#nullable enable
using System;

namespace SemBackDotNet10.Common.exception;

public class InvalidStateException : Exception
{
    public InvalidStateException(string? message) : base(message) { }
}
