using System.Text.Json.Nodes;
namespace Common.logging;
public sealed class LogService(IElasticsearchSearchTransport transport,SemLogger logger)
{
    public static JsonObject BuildSearchRequest(string body,int page,int size)
    {
        if(page<0 || size<0)throw new ArgumentOutOfRangeException(nameof(page),"page and size must be non-negative.");
        var input=JsonNode.Parse(body) as JsonObject ?? throw new FormatException("Expected a JSON object.");
        string? Read(string key)=>input[key]?.GetValue<string>();
        var must=new JsonArray();
        foreach(var (key,field,kind) in new[]{("search","message","match_phrase"),("logLevel","level","match"),("IP","caller_ip_address","match"),("userName","user_name","match")})
        {
            var value=Read(key);
            if(!string.IsNullOrEmpty(value))must.Add(new JsonObject{[kind]=new JsonObject{["jsoncontent."+field]=new JsonObject{["query"]=value}}});
        }
        var boolean=new JsonObject();
        if(must.Count>0)boolean["must"]=must;
        var start=Read("startDate");var end=Read("endDate");
        if(start is not null && end is not null)boolean["filter"]=new JsonArray(new JsonObject{["range"]=new JsonObject{
            ["jsoncontent.timestamp"]=new JsonObject{["gte"]=start,["lte"]=end}}});
        return new JsonObject{["query"]=new JsonObject{["bool"]=boolean},["from"]=checked(page*size),["size"]=size,
            ["sort"]=new JsonArray(new JsonObject{["jsoncontent.timestamp"]=new JsonObject{["order"]="desc"}})};
    }
    public async Task<List<JsonObject?>> GetFilteredLogs(string body,int page,int size,string? orderBy,string? direction,CancellationToken cancellationToken=default)
    {
        logger.Info("Enter the method");
        try
        {
            // Java intentionally ignores orderBy/direction and always sorts timestamp descending.
            var response=await transport.SearchAsync("logstash-*",BuildSearchRequest(body,page,size),cancellationToken);
            var hits=response["hits"]?["hits"] as JsonArray ?? throw new InvalidDataException("Missing search hits.");
            var result=hits.Select(hit=>hit?["_source"]?["jsoncontent"]?.DeepClone() as JsonObject).ToList();
            logger.Info("Exit method");return result;
        }
        catch {logger.Error("an error occured in fetching log ");throw;}
    }
}
