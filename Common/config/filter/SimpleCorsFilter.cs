using Microsoft.Extensions.DependencyInjection;
namespace Common.config.filter;

// Intentional security correction: the Java filter reflects ANY origin when its list
// is nonempty and combines it with credentials. This port uses an exact allowlist.
public static class SimpleCorsFilter
{
    public const string PolicyName="SemCors";
    public static IServiceCollection AddSemCors(this IServiceCollection services, IEnumerable<string> allowedOrigins,
        IEnumerable<string> allowedMethods, IEnumerable<string> allowedHeaders, TimeSpan maxAge)
    {
        var origins=allowedOrigins.Select(x=>x.Trim()).Where(x=>x.Length>0).Distinct(StringComparer.Ordinal).ToArray();
        if(origins.Length==0 || origins.Any(x=>x=="*" || !Uri.TryCreate(x,UriKind.Absolute,out var u) || (u.Scheme!="http" && u.Scheme!="https") || u.GetLeftPart(UriPartial.Authority)!=x))
            throw new ArgumentException("Explicit HTTP(S) origins without paths or trailing slashes are required.",nameof(allowedOrigins));
        if(maxAge<TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(maxAge));
        var methods=allowedMethods.ToArray(); var headers=allowedHeaders.ToArray();
        if(methods.Length==0 || headers.Length==0) throw new ArgumentException("Allowed methods and headers must be explicit.");
        return services.AddCors(options=>options.AddPolicy(PolicyName,builder=>builder.WithOrigins(origins)
            .WithMethods(methods).WithHeaders(headers).AllowCredentials().SetPreflightMaxAge(maxAge)));
    }
}
