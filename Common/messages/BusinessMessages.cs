// C# migration of messages/BusinessMessages.java

namespace Common.messages;

public static class BusinessMessages
{
    public const string DATA_INCONSISTENT = "خطایی در دریافت یا پردازش اطلاعات رخ داد.";
    public const string CATALOG_ID_NULL = "شناسه کاتالوگ نمی‌تواند خالی باشد.";
    public const string PLAN_DATA_NOT_FOUND = "هیچ داده‌ای برای کاتالوگ مورد نظر یافت نشد.";
    public const string INCONSISTENT_YEAR_PLAN_DATA = "مقادیر درصد شروع سال یا درصد هدف سال جاری بین برنامه‌ ریزی های اقدام یکسان نیستند.";
    public const string RECORD_NOT_FOUND_BY_ID = "رکورد با شناسه مورد نظر یافت نشد.";
    public const string UPDATE_FAILED = "خطایی در به‌روزرسانی %s رخ داد!";
    public const string CARTABLE_PROPAGATION_FAILED = "خطایی در انتقال تغییرات به سطوح بالاتر رخ داد!";
    public const string CARTABLE_DATA_INCONSISTENT = "خطا در بازیابی اطلاعات مرتبط رخ داد!";
    public const string CARTABLE_INVALID_STATE = "وضعیت فعلی معتبر نمی‌باشد!";
    public const string ACTION_PLAN_NOT_PLANNED = "اتصال اقدام استراتژیک به هدف برنامه‌ریزی نشده است";
    public const string DATA_FETCH_FAILED = "خطایی در دریافت اطلاعات رخ داد!";
    public const string MEASURE_NOT_PLANNED = "اتصال شاخص استراتژیک به هدف برنامه‌ریزی نشده است.";
    public const string MEASURE_NOT_FOUND = "شاخصی با شناسه مورد نظر یافت نشد.";
    public const string MEASURE_FETCH_FAILED = "خطایی در دریافت اطلاعات شاخص‌ها رخ داد.";
    public const string MEASURE_ACT_PLAN_DELETE_FAILED = "خطا در حذف برنامه ریزی شاخص رخ داد.";
    public const string MEASURE_DELETE_FAILED = "خطا در حذف اتصال شاخص به هدف رخ داد.";
    public const string MEASURE_REPOSITORY_DELETE_FAILED = "خطا در حذف اطلاعات شاخص رخ داد.";
    public const string MEASURE_ACT_PLAN_RESTORE_FAILED = "خطا در بازیابی برنامه‌های شاخص رخ داد.";
    public const string CARTABLE_RESTORE_FAILED = "خطا در بازگردانی وضعیت رخ داد.";
    public const string EMPTY_MEASURE_ID_LIST_NOT_VALID = "لیست شناسه شاخص‌ها نباید خالی باشد!";
    public const string EMPTY_MEASURE_LIST_NOT_VALID = "لیست شاخص‌ها نباید خالی باشد!";
    public const string EMPTY_MEASURE_ID_NOT_VALID = "شناسه شاخص‌ نباید خالی باشد!";
    public const string EMPTY_ACTION_PLAN_CATALOG_ID_NOT_VALID = "شناسه کاتالوگ اقدام نباید خالی باشد!";
    public const string EMPTY_MONTH_NOT_VALID = "ماه مورد نظر نباید خالی باشد!";
    public const string MEASURE_CHECK_PLANNED_FAILED = "خطا در بررسی وضعیت برنامه‌ریزی شاخص رخ داد.";
    public const string DASHBOARD_DELETE_FAILED = "خطا در حذف اطلاعات داشبورد رخ داد.";
    public const string YEAR_TARGET_NOT_VALID = "مقدار وارد شده برای درصد تکمیل سال جاری معتبر نمی باشد!";
    public const string INVALID_MONTH_PLAN_DIRECTION = "ترتیب مقادیر برنامه‌ریزی ماه با روند انتخاب‌شده همخوانی ندارد.";
    public const string UNEXPECTED_EXCEPTION = "خطای غیرمنتظره در %s رخ داده است.";
    public const string ERROR_DELETE_MEASURE_CATALOG_RELATIONS = "خطایی در فرآیند حذف روابط محاسباتی شاخص رخ داده است.";
    public const string ERROR_REMOVING_MEASURE_CATALOGS = "خطا در حذف شاخص‌ها رخ داده است.";
    public const string EMPTY_FILE_NOT_VALID = "فایل آپلود شده نمیتواند خالی باشد!";
    public const string INVALID_FILE_TYPE = "نوع فایل آپلود شده معتبر نمی باشد!";
    public const string ATTACHMENT_FILE_READ_ERROR = "خطا در خواندن فایل بارگذاری‌شده رخ داده است.";
    public const string ATTACHMENT_FILE_SAVE_ERROR = "خطا در ذخیره فایل بارگذاری‌شده رخ داده است.";
    public const string ATTACHMENT_FILE_DOWNLOAD_ERROR = "خطا در دانلود فایل بارگذاری‌شده رخ داده است.";
    public const string STORED_FILE_NOT_FOUND = "هیچ داده‌ای برای فایل اپلود شده مورد نظر یافت نشد.";
}
