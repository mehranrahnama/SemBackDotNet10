using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Text;
using Common.utils;
namespace Common.config.modificationRequest;

// Normalizes body/query/form values. Does NOT rewrite Path or signed raw query strings.
// Apply after signature verification and before model binding. Host request-size limits apply.
public static class ModificationRequestWrapper
{
    public static async Task NormalizeAsync(HttpRequest request, CancellationToken cancellationToken=default)
    {
        request.Query=new QueryCollection(request.Query.ToDictionary(p=>p.Key,p=>Normalize(p.Value)));
        if(ContentModifiedPart.IsJson(request.ContentType))
        {
            var encoding=Encoding.UTF8;
            var charset=request.ContentType?.Split(';').Select(x=>x.Trim()).FirstOrDefault(x=>x.StartsWith("charset=",StringComparison.OrdinalIgnoreCase));
            if(charset is not null) encoding=Encoding.GetEncoding(charset[8..].Trim('"'));
            using var reader=new StreamReader(request.Body,encoding,detectEncodingFromByteOrderMarks:true,leaveOpen:true);
            var text=await reader.ReadToEndAsync(cancellationToken);
            var data=encoding.GetBytes(string.IsNullOrWhiteSpace(text) ? text : StringUtil.ReplaceSpecialCharacter(text) ?? text);
            request.Body=new MemoryStream(data,writable:false);
            request.ContentLength=data.Length;
        }
        if(request.HasFormContentType)
        {
            var form=await request.ReadFormAsync(cancellationToken);
            var files=new FormFileCollection();
            foreach(var file in form.Files) files.Add(await ContentModifiedPart.CreateAsync(file,cancellationToken:cancellationToken));
            request.Form=new FormCollection(form.ToDictionary(p=>p.Key,p=>Normalize(p.Value)),files);
        }
    }
    private static StringValues Normalize(StringValues values) => new(values.Select(x=>x is null ? null : StringUtil.ReplaceSpecialCharacter(x)).ToArray());
}
