using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SemBackDotNet10.SecurityDomain.Configuration;
using SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;

namespace SemBackDotNet10.SecurityDomain.Handler;

public sealed class OAuth2AuthenticationFailureHandler(
    IOptions<OAuth2Options> options,
    ILogger<OAuth2AuthenticationFailureHandler> logger)
{
    private readonly OAuth2Options _options = options.Value;

    public Task OnRemoteFailureAsync(RemoteFailureContext context)
    {
        logger.LogInformation("OAuth2 authentication failure was received.");
        var target = AuthenticationCookieUtils.Get(
            context.Request,
            AuthenticationCookieUtils.RedirectUriCookieName) ?? _options.DefaultRedirectUri;
        var error = context.Failure?.GetBaseException().Message ?? "authentication_failed";
        var redirect = QueryHelpers.AddQueryString(target, "error", error);

        AuthenticationCookieUtils.Delete(
            context.Response,
            AuthenticationCookieUtils.RedirectUriCookieName);
        context.Response.Redirect(redirect);
        return Task.CompletedTask;
    }

    public Task HandleAsync(RemoteFailureContext context) => OnRemoteFailureAsync(context);
}
