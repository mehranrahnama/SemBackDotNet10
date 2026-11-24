#nullable enable
using System;
using Microsoft.AspNetCore.Http;

namespace SemBackDotNet10.Common.exception.handler;

public sealed class CustomizedExceptionHandlerOptions
{
    // Must match the Path and Domain used when issuing sessionKey.
    public string SessionCookiePath { get; set; } = "/";
    public string? SessionCookieDomain { get; set; }
    public bool SessionCookieSecure { get; set; } = true;
    public SameSiteMode SessionCookieSameSite { get; set; } = SameSiteMode.Lax;

    // Do not disclose internal server IPs by default.
    public bool IncludeServerAddress { get; set; }

    // Optional classifier for provider/ORM-specific exceptions, e.g. DbUpdateException.
    public Func<Exception, bool>? IsDatabaseException { get; set; }
}
