namespace Common.utils;

public sealed class LocaleUtils(Microsoft.Extensions.Configuration.IConfiguration configuration)
{
 public System.Globalization.CultureInfo GetInstance() {
  string? locale=configuration["Security:Locale"] ?? configuration["sem:locale"] ?? configuration["sem.locale"];
  if(locale is null)throw new global::SemBackDotNet10.Common.exception.ServiceException("error.localeUtils.localeNotSetInSemConfig");
  return System.Globalization.CultureInfo.GetCultureInfo(locale.Replace('_','-'));
 }
}
