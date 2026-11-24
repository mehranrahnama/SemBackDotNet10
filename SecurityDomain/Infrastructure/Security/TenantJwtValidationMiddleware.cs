using SemBackDotNet10.SecurityDomain.Tenancy;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Security;

public sealed class TenantJwtValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("tenant_id")?.Value;
            if (!long.TryParse(tenantClaim, out var tokenTenantId) ||
                !tenantContext.HasTenant || tokenTenantId != tenantContext.TenantId)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    code = "TENANT_MISMATCH",
                    message = "The token tenant does not match the request tenant."
                });
                return;
            }
        }

        await next(context);
    }
}
