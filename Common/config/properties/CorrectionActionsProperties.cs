// C# migration of config/properties/CorrectionActionsProperties.java

namespace Common.config.properties;

public class CorrectionActionsProperties
{
 public const string SectionName="correction";
 public string? Url { get; set; }
 public QueryParamsOptions? QueryParams { get; set; }
 public class QueryParamsOptions {
 public string? SystemStandardCode { get; set; }
 public string? ActionOriginCode { get; set; }
 public string? IdentityCode { get; set; }
 public string? Status { get; set; }
 public string? ShowInactives { get; set; }
 public string? Date { get; set; }
 }
}
