// C# migration of utils/RequestInfoTwoParam.java

namespace Common.utils;

public class RequestInfoTwoParam<T,D>
{
 public T? Param1 { get; set; }
 public D? Param2 { get; set; }
 public RequestInfoTwoParam() { }
 public RequestInfoTwoParam(T param1,D param2) { Param1=param1;Param2=param2; }
}
