using System.Net.Http.Headers;
namespace Common.webServices.ErpService;
public static class ErpServicesHelper
{
    public static string CreateUrl(int page,int size,string serviceName)
    {
        var path=serviceName switch {
            "EMPLOYEE"=>"sql2cs/do/erp_employees", "PROJECTS"=>"sql2cs/do/erp_projects",
            "ORGANIZATIONS"=>"sql2cs/do/erp_organizations", "SMS"=>"nc/do/sms_service",
            _=>throw new ArgumentException("Unknown ERP service.",nameof(serviceName))
        };
        return $"https://frame.irisaco.com/api/{path}/rest/v1.0?pageNumber={page}&pageSize={size}";
    }
    public static string CreateSmsServiceUrl()=>"https://frame.irisaco.com/api/nc/do/sms_service/rest/v1.0";
    public static string CreateResetPasswordSmsMessage(string message)=>$"«مدیریت استراتژیک راهبرد»\nکاربر عزیز،رمز عبور شما ریست شد!\nجهت ورود به اپلیکیشن از رمز عبور زیر استفاده نمایید.\nرمز عبور شما:\n{message}\nلطفا نسبت به تغییر رمز عبور خود اقدام فرمایید.\nhttps://semgroup.irisaco.com\nلغو 11\n\n";
    // Preserves Java's literal %s: its source never calls formatted(message).
    public static string CreateMyIrisaMessage(string message)=>"کاربر عزیز،رمز عبور شما تغییر یافت!\nجهت ورود به اپلیکیشن از رمز عبور زیر استفاده نمایید.\nرمز عبور شما:\n%s\nلطفا نسبت به تغییر رمز عبور خود اقدام فرمایید.\nhttps://kms.irisaco.com\nلغو 11\n";
    public static string CreateMyIrisaMessageForOTP(string message)=>$"رمز یکبار مصرف شما:\n{message}\nلغو 11\n";
    public static string CreateSmsMessageForNewUsers(string user,string password)=>$"«مدیریت استراتژیک راهبرد»\nضمن عرض خوش ‌آمد گویی\nحساب کاربری شما در سامانه راهبرد ایجاد گردید!\nجهت ورود به سامانه\nاز اطلاعات زیر استفاده نمایید.\nنام کاربری:\n{user}\nرمز عبور:\n{password}\n لطفا نسبت به تغییر رمز عبور خود اقدام فرمایید.\n https://semgroup.irisaco.com\n لغو 11\n";
    public static Dictionary<string,object> CreateSmsBody(string message,IReadOnlyList<string> numbers)=>new(){{"Message",message},{"Numbers",numbers}};
    public static List<string> CreateSmsNumbers(IReadOnlyList<string> numbers)=>numbers.Select(n=>"98"+n[1..]).ToList();
    public static AuthenticationHeaderValue SetTokenHeader(string token)=>new("Bearer",token);
}
