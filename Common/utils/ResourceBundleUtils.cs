namespace Common.utils;

public sealed class ResourceBundleUtils(System.Resources.ResourceManager messages,LocaleUtils locales)
{
 public string GetMessage(string code,params object?[] args) {
  ArgumentNullException.ThrowIfNull(code);
  string? value;
  try { value=messages.GetString(code,locales.GetInstance()); }
  catch(System.Resources.MissingManifestResourceException) { value=null; }
  // Java useCodeAsDefaultMessage returns the code verbatim if not found.
  if(value is null)return code;
  return args.Length==0?value:string.Format(locales.GetInstance(),value,args);
 }
}
