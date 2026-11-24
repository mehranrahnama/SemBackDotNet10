#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.Common.Http;

/// <summary>
/// Optional: reproduces only the explicit @ResponseStatus annotations in the supplied Java.
/// Response body is an integration choice, not a Java handler conversion.
/// </summary>
public sealed class JavaResponseStatusExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            ServiceException => StatusCodes.Status417ExpectationFailed,
            CustomException => StatusCodes.Status417ExpectationFailed,
            InvalidStateException => StatusCodes.Status422UnprocessableEntity,
            _ => 0
        };

        if (status == 0 || httpContext.Response.HasStarted)
            return false;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(
            new ErrorResponse(exception.Message), cancellationToken);
        return true;
    }
}
