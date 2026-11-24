// C# migration of utils/StringUtil.java

namespace Common.utils;

public static class StringUtil
{
 public static string? ReplaceSpecialCharacter(string? input) => input?.Replace('ك','ک').Replace('ى','ی').Replace('ي','ی');
 public static string ApplyParameter(string source, IReadOnlyDictionary<string,string>? parameters) {
  ArgumentNullException.ThrowIfNull(source);
  if(parameters is null || parameters.Count==0) return source;
  return System.Text.RegularExpressions.Regex.Replace(source, @"\{([^{}]+)\}", m => parameters.TryGetValue(m.Groups[1].Value,out var value) ? value : m.Value);
 }
}
