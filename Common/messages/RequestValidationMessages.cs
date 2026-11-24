// C# migration of messages/RequestValidationMessages.java

namespace Common.messages;

public static class RequestValidationMessages
{
    public const string PLAN_ID_REQUIRED = "سال برنامه ریزی الزامی است.";
    public const string PLAN_ID_POSITIVE = "سال برنامه ریزی باید بزرگتر از صفر باشد.";
    public const string CATALOG_ID_REQUIRED = "شناسه کاتالوگ اقدام الزامی است.";
    public const string CATALOG_ID_POSITIVE = "شناسه کاتالوگ اقدام باید بزرگتر از صفر باشد.";
    public const string ENTITY_NAME_REQUIRED = "فیلد نام منبع آپلود الزامی است.";
    public const string ENTITY_ID_REQUIRED = "شناسه منبع آپلود الزامی است.";
    public const string ENTITY_ID_MUST_BE_POSITIVE = "شناسه موجودیت باید عددی بزرگ‌تر از صفر باشد.";
    public const string DESCRIPTION_REQUIRED = "توضیحات الزامی است.";
    public const string DESCRIPTION_SIZE_INVALID = "توضیحات باید بین ۳ تا ۵۰۰ کاراکتر باشد.";
}
