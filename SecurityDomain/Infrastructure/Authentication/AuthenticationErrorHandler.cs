using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;

public static class AuthenticationErrorHandler
{
    public static void Configure(JwtBearerOptions options)
    {
        options.Events ??= new JwtBearerEvents();
        options.Events.OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                code = "UNAUTHORIZED",
                message = "Authentication required."
            });
        };

        options.Events.OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                code = "FORBIDDEN",
                message = "Access denied."
            });
        };
    }
}
