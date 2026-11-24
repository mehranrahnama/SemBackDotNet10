// C# migration of utils/DateUtils.java

namespace Common.utils;

public static class DateUtils
{
 public static int CompareDate(string date1,string date2) {
  var a=date1.Split('/').Select(int.Parse).ToArray(); var b=date2.Split('/').Select(int.Parse).ToArray();
  for(int i=0;i<3;i++) {int c=a[i].CompareTo(b[i]);if(c!=0)return c;} return 0;
 }
}
