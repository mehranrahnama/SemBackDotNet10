using Microsoft.AspNetCore.Http;
using Common.utils;
using System.Text;
namespace Common.config.modificationRequest;

// Explicit UTF-8 by default rather than the platform-dependent Java getBytes().
// Binary and non-JSON uploads are copied byte-for-byte.
public static class ContentModifiedPart
{
    public static async Task<IFormFile> CreateAsync(IFormFile original, Encoding? encoding=null, CancellationToken cancellationToken=default)
    {
        ArgumentNullException.ThrowIfNull(original);
        encoding ??= Encoding.UTF8;
        using var input=original.OpenReadStream();
        using var output=new MemoryStream();
        if(IsJson(original.ContentType))
        {
            using var reader=new StreamReader(input,encoding,detectEncodingFromByteOrderMarks:true,leaveOpen:true);
            var text=await reader.ReadToEndAsync(cancellationToken);
            var bytes=encoding.GetBytes(string.IsNullOrWhiteSpace(text) ? text : StringUtil.ReplaceSpecialCharacter(text) ?? text);
            await output.WriteAsync(bytes,cancellationToken);
        }
        else await input.CopyToAsync(output,cancellationToken);
        var data=output.ToArray();
        return new FormFile(new MemoryStream(data,writable:false),0,data.Length,original.Name,original.FileName)
        { Headers=new HeaderDictionary(original.Headers.ToDictionary(x=>x.Key,x=>x.Value)), ContentType=original.ContentType, ContentDisposition=original.ContentDisposition };
    }
    internal static bool IsJson(string? contentType)
    {
        var media=contentType?.Split(';',2)[0].Trim();
        return string.Equals(media,"application/json",StringComparison.OrdinalIgnoreCase) || (media?.EndsWith("+json",StringComparison.OrdinalIgnoreCase)??false);
    }
}
