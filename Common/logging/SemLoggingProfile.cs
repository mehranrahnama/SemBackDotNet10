using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
namespace Common.logging;
// Host: mvc.ConfigureApplicationPartManager(m=>m.FeatureProviders.Add(new SemLoggingProfile(isDevProfile)));
// Only register Elasticsearch/LogService when isDevProfile=false. 'dev' is not automatically .NET Development.
public sealed class SemLoggingProfile(bool isDevProfile):IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts,ControllerFeature feature)
    { if(isDevProfile)feature.Controllers.Remove(typeof(LogController).GetTypeInfo()); }
}
