#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.Common.exception.handler;

public sealed class CustomizedExceptionHandler(
    ILogger<CustomizedExceptionHandler> logger,
    IStringLocalizer<CustomizedExceptionHandler> localizer,
    IOptions<CustomizedExceptionHandlerOptions> options) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (context.Response.HasStarted)
            return false;
        if (exception is OperationCanceledException && context.RequestAborted.IsCancellationRequested)
            return false;

        int status;
        object body;
        switch (exception)
        {
            case BusinessException business:
                status = StatusCodes.Status417ExpectationFailed;
                var args = business.Args?.Cast<object>().ToArray() ?? Array.Empty<object>();
                body = Details(context, Translate(business.Message, args), business.Data);
                break;
            case InvalidStateException invalidState:
                status = StatusCodes.Status422UnprocessableEntity;
                body = Details(context, invalidState.Message);
                break;
            case ServiceException service:
                status = StatusCodes.Status417ExpectationFailed;
                body = Details(context, Translate(service.Message));
                break;
            case AccessForbiddenException forbidden:
                status = StatusCodes.Status403Forbidden;
                body = new ErrorResponse { Message = forbidden.Message };
                break;
            case UnAuthorizedException:
                status = StatusCodes.Status401Unauthorized;
                body = new ErrorResponse { Message = "عدم مجوز دسترسی" };
                break;
            case TokenRefreshException refresh:
                status = StatusCodes.Status401Unauthorized;
                if (context.Request.Cookies.ContainsKey("sessionKey"))
                {
                    context.Response.Cookies.Delete("sessionKey", new CookieOptions
                    {
                        Path = options.Value.SessionCookiePath,
                        Domain = options.Value.SessionCookieDomain,
                        Secure = options.Value.SessionCookieSecure,
                        HttpOnly = true,
                        SameSite = options.Value.SessionCookieSameSite
                    });
                }
                body = new ErrorResponse { Message = refresh.Message };
                break;
            case BruteForceException bruteForce:
                status = StatusCodes.Status423Locked;
                // Java puts Params in additionalData, not LockTime/LockStatus.
                body = new ErrorDetails(new List<string> { bruteForce.Message }, bruteForce.Params);
                break;
            case ValidationException validation:
                status = StatusCodes.Status400BadRequest;
                var members = validation.ValidationResult?.MemberNames ?? Array.Empty<string>();
                var errors = members.Select(name => name + ": " + validation.Message).ToList();
                if (errors.Count == 0)
                    errors.Add(validation.Message);
                body = new ErrorResponse("Data Validation Failed", errors);
                break;
            case BadHttpRequestException badRequest:
                status = badRequest.StatusCode;
                body = new ErrorResponse("Invalid request");
                break;
            default:
                var isDatabase = exception is DbException ||
                    exception.GetBaseException() is DbException ||
                    options.Value.IsDatabaseException?.Invoke(exception) == true;
                status = isDatabase
                    ? StatusCodes.Status417ExpectationFailed
                    : StatusCodes.Status500InternalServerError;
                body = Details(context,
                    Translate(isDatabase ? "خطای پایگاه داده" : "خطای زمان اجرا"),
                    currentUser: context.User.Identity?.IsAuthenticated == true
                        ? context.User.Identity.Name : null);
                logger.LogError(exception,
                    "Unhandled request exception. TraceIdentifier: {TraceIdentifier}",
                    context.TraceIdentifier);
                break;
        }

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(body, cancellationToken);
        return true;
    }

    private string Translate(string? message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return string.Empty;
        var localized = localizer[message];
        if (localized.ResourceNotFound) return message;
        return args.Length == 0 ? localized.Value : localizer[message, args].Value;
    }

    private ErrorDetails Details(HttpContext context, string message,
        object? data = null, string? currentUser = null) => new(
            new List<string> { message }, DateTime.Now,
            "uri=" + context.Request.Path,
            currentUser,
            options.Value.IncludeServerAddress ? context.Connection.LocalIpAddress?.ToString() : null,
            data, null);
}
