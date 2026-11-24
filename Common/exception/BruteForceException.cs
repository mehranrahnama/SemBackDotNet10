#nullable enable
using System;
using System.Collections.Generic;

namespace SemBackDotNet10.Common.exception;

public class BruteForceException : Exception
{
    public Dictionary<string, object?>? Params { get; }
    public DateTime? LockTime { get; set; }
    public bool? LockStatus { get; set; }

    public BruteForceException(string? message, DateTime? lockTime, bool? lockStatus)
        : base(message)
    {
        Params = new Dictionary<string, object?>();
        LockTime = lockTime;
        LockStatus = lockStatus;
    }

    public BruteForceException(string? message, Dictionary<string, object?>? parameters)
        : base(message)
    {
        Params = parameters;
    }
}
