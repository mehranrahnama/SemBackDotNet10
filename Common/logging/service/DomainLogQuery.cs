using System.Text.Json.Nodes;
namespace Common.logging.service;
// Register only outside the dev profile, like the Java services.
public static class DomainLogQuery
{
    public static JsonObject Build(string body,string nameKey,string method,string file,bool sort)
    {
        var input=JsonNode.Parse(body) as JsonObject ?? throw new FormatException("Expected JSON object.");
        var name=input[nameKey]?.GetValue<string>();
        var clauses=new JsonArray();
        if(!string.IsNullOrEmpty(name))
        {
            clauses.Add(new JsonObject{["match_phrase"]=new JsonObject{["jsoncontent.message"]=new JsonObject{["query"]=name}}});
            clauses.Add(new JsonObject{["match"]=new JsonObject{["jsoncontent.caller_method_name"]=new JsonObject{["query"]=method}}});
            clauses.Add(new JsonObject{["match"]=new JsonObject{["jsoncontent.caller_file_name"]=new JsonObject{["query"]=file}}});
        }
        var boolean=new JsonObject();if(clauses.Count>0)boolean["must"]=clauses;
        var query=new JsonObject{["query"]=new JsonObject{["bool"]=boolean}};
        if(sort)query["sort"]=new JsonArray(new JsonObject{["jsoncontent.timestamp"]=new JsonObject{["order"]="desc"}});
        return query;
    }
    public static async Task<List<JsonObject?>> Fetch(IElasticsearchSearchTransport transport,JsonObject query,CancellationToken cancellationToken)
    {
        var response=await transport.SearchAsync("logstash-*",query,cancellationToken);
        var hits=response["hits"]?["hits"] as JsonArray ?? throw new InvalidDataException("Missing search hits.");
        return hits.Select(h=>h?["_source"]?["jsoncontent"]?.DeepClone() as JsonObject).ToList();
    }
}
