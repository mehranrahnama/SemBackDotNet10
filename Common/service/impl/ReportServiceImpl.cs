using System.Globalization;
using System.Text.Json;
using Common.service.interf;
namespace Common.service.impl;

public sealed record ReportRequest(string Name,string Format,long? LevelId,long? PlanId);
public interface IReportExecutionHost
{
    // Must enforce current-user access BEFORE queries for BOTH report names.
    // Host owns SQL/SP mapping, createdBy, Persian date/timezone, Jasper-compatible
    // templates, rendering, and safe publication. No default implementation is supplied.
    void Authorize(ReportRequest request);
    byte[] Render(ReportRequest request);
    string Publish(ReportRequest request,byte[] content);
}
public sealed class ReportServiceImpl(IReportExecutionHost host) : ReportService
{
    public static ReportRequest ParseRequest(string? filter,string fileFormat,string reportName)
    {
        ArgumentNullException.ThrowIfNull(fileFormat);
        ArgumentNullException.ThrowIfNull(reportName);
        var format=fileFormat.ToLowerInvariant();
        if(format is not ("pdf" or "excel"))throw new ArgumentException("Expected pdf or excel.",nameof(fileFormat));
        var name=reportName.Equals("PlaningLevels",StringComparison.OrdinalIgnoreCase)?"PlaningLevels":
            reportName.Equals("ObjectActions",StringComparison.OrdinalIgnoreCase)?"ObjectActions":
            throw new ArgumentException("Unsupported report name.",nameof(reportName));
        if(name=="PlaningLevels")return new(name,format,null,null);
        using var json=JsonDocument.Parse(filter??throw new ArgumentNullException(nameof(filter)));
        if(json.RootElement.ValueKind!=JsonValueKind.Object)throw new FormatException("Expected JSON object.");
        long? Id(string key)
        {
            var value=json.RootElement.GetProperty(key);
            if(value.ValueKind==JsonValueKind.Null)return null;
            var text=value.ValueKind==JsonValueKind.String?value.GetString():value.GetRawText();
            return text=="null"?null:long.Parse(text!,NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture);
        }
        return new(name,format,Id("levelId"),Id("planId"));
    }
    public string GenerateReport(string? filter,string fileFormat,string reportName)
    {
        var request=ParseRequest(filter,fileFormat,reportName);
        host.Authorize(request);
        return host.Publish(request,host.Render(request));
    }
    public byte[] GenerateReportAsByteArray(string? filter,string fileFormat,string reportName)
    {
        var request=ParseRequest(filter,fileFormat,reportName);
        // Java returns null for non-PDF bytes; reject explicitly rather than return a misleading empty file.
        if(request.Format!="pdf")throw new NotSupportedException("Byte-array reports support PDF only.");
        host.Authorize(request);
        return host.Render(request);
    }
}
