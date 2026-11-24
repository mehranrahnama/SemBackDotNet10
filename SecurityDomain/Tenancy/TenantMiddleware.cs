namespace SemBackDotNet10.SecurityDomain.Tenancy
{
    public sealed class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITenantResolver tenantResolver,
            TenantContext tenantContext)
        {
            var host = context.Request.Host.Host;

            long tenantId;
            try
            {
                tenantId = await tenantResolver.ResolveTenantIdAsync(host, context.RequestAborted);
            }
            catch (TenantResolutionException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "TENANT_NOT_FOUND",
                    message = "Tenant could not be resolved."
                });
                return;
            }

            tenantContext.SetTenant(tenantId);
            await _next(context);
        }
    }
}
