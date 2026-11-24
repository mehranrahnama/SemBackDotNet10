#nullable enable
using System;
using System.Collections.Generic;

namespace SemBackDotNet10.Common.exception;

public class ErrorDetails
{
    public List<string>? Message { get; set; }
    public DateTime? TimeStamp { get; set; }
    public string? Details { get; set; }
    public string? CurrentUser { get; set; }
    public string? ServerAddress { get; set; }
    public object? AdditionalData { get; set; }
    public string? ErrorCode { get; set; }

    public ErrorDetails() { }

    public ErrorDetails(List<string>? message, DateTime? timeStamp, string? details,
        string? currentUser, string? serverAddress)
        : this(message, timeStamp, details, currentUser, serverAddress, null, null) { }

    public ErrorDetails(List<string>? message, DateTime? timeStamp, string? details,
        string? currentUser, string? serverAddress, object? additionalData, string? errorCode)
    {
        Message = message;
        TimeStamp = timeStamp;
        Details = details;
        CurrentUser = currentUser;
        ServerAddress = serverAddress;
        AdditionalData = additionalData;
        ErrorCode = errorCode;
    }

    public ErrorDetails(List<string>? message, object? additionalData)
    {
        Message = message;
        AdditionalData = additionalData;
    }
}

// C# does not allow a nested enum and property both named ErrorCode.
// Kept at namespace scope; persist/serialize the Java code as a string.
public enum ErrorCode
{
    GENERAL_ERROR
}
