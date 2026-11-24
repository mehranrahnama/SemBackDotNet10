using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SemBackDotNet10.SecurityDomain.Handler;

/// <summary>JWT bearer challenge handler equivalent to the Java AuthenticationEntryPoint.</summary>
public sealed class UnAuthorizedHandler
{
    public async Task OnChallengeAsync(JwtBearerChallengeContext context)
    {
        context.HandleResponse();
        var response = context.HttpContext.Response;
        var message = context.AuthenticateFailure?.Message ?? context.Error ?? "Unauthorized";
        var status = context.AuthenticateFailure is SecurityTokenExpiredException
            ? StatusCodes.Status401Unauthorized
            : StatusCodes.Status403Forbidden;
        var error = status == StatusCodes.Status401Unauthorized ? "token.Expired" : message;

        response.StatusCode = status;
        await response.WriteAsJsonAsync(new { status, error });
    }
}
