using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
namespace Common.logging;
public sealed class SemLogger(ILoggerFactory factory,IHttpContextAccessor http)
{
    private void Write(LogLevel level,object? message,string member,string file,int line)
    {
        var context=http.HttpContext;
        var user=context is null?"system":context.User.Identity?.IsAuthenticated==true?context.User.Identity.Name??"":"anonymousUser";
        var logger=factory.CreateLogger(Path.GetFileNameWithoutExtension(file));
        using var scope=logger.BeginScope(new Dictionary<string,object?>{
            ["user_name"]=user,["caller_class_name"]=Path.GetFileNameWithoutExtension(file),
            ["caller_method_name"]=member,["caller_file_name"]=Path.GetFileName(file),
            ["caller_line_number"]=line.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["caller_ip_address"]=context?.Connection.RemoteIpAddress?.ToString()??""});
        logger.Log(level,"{Message}",message?.ToString()??"");
    }
    public void Info(object? message,[CallerMemberName]string member="",[CallerFilePath]string file="",[CallerLineNumber]int line=0)
        =>Write(LogLevel.Information,message,member,file,line);
    public void Debug(object? message,[CallerMemberName]string member="",[CallerFilePath]string file="",[CallerLineNumber]int line=0)
        =>Write(LogLevel.Debug,message,member,file,line);
    public void Error(object? message,[CallerMemberName]string member="",[CallerFilePath]string file="",[CallerLineNumber]int line=0)
        =>Write(LogLevel.Error,message,member,file,line);
    public void Trace(object? message,[CallerMemberName]string member="",[CallerFilePath]string file="",[CallerLineNumber]int line=0)
        =>Write(LogLevel.Trace,message,member,file,line);
    public void Warn(object? message,[CallerMemberName]string member="",[CallerFilePath]string file="",[CallerLineNumber]int line=0)
        =>Write(LogLevel.Warning,message,member,file,line);
}
