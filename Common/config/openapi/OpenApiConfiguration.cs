using System.Text.Json.Nodes;
namespace Common.config.openapi;
// Adapter for a generated OpenAPI JSON document; host must call Apply after generation.
public static class OpenApiConfiguration
{
    public static void Apply(JsonObject document,string serverUrl="http://localhost:8080/sem")
    {
        var info=document["info"] as JsonObject ?? new JsonObject(); document["info"]=info;info["version"]="1.0.0";
        document["servers"]=new JsonArray(new JsonObject{["url"]=serverUrl,["description"]="Dev environment"});
        var components=document["components"] as JsonObject ?? new JsonObject();document["components"]=components;
        var schemes=components["securitySchemes"] as JsonObject ?? new JsonObject();components["securitySchemes"]=schemes;
        schemes["Bearer Auth"]=new JsonObject{["type"]="http",["scheme"]="bearer",["bearerFormat"]="JWT"};
        document["security"]=new JsonArray(new JsonObject{["Bearer Auth"]=new JsonArray()});
        if(document["paths"] is not JsonObject paths)return;
        string[] verbs=["get","put","post","delete","options","head","patch","trace"];
        foreach(var path in paths.Select(p=>p.Value).OfType<JsonObject>())
        foreach(var verb in verbs)
        {
            if(path[verb] is not JsonObject operation)continue;
            var parameters=operation["parameters"] as JsonArray ?? new JsonArray();operation["parameters"]=parameters;
            if(parameters.OfType<JsonObject>().Any(p=>p["name"]?.ToString()=="factoryId" && p["in"]?.ToString()=="header"))continue;
            parameters.Add(new JsonObject{["name"]="factoryId",["in"]="header",["description"]="Will Raise Error if The Service needs it",["required"]=false,["schema"]=new JsonObject{["type"]="integer",["format"]="int32"}});
        }
    }
}
