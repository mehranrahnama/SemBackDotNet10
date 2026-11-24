// C# migration of utils/cartable/CartableTaskPrefix.java

namespace Common.utils.cartable;

public static class CartableTaskPrefix
{
    public const string CREATE_MEASURE_CATALOG = "تایید شاخص استراتژیک";
    public const string DELETE_MEASURE_CATALOG = "تایید حذف  شاخص استراتژیک";
    public const string CREATE_MEASURE = "تایید برنامه ریزی شاخص استراتژیک";
    public const string CREATE_MEASURE_REPLAN = "تایید برنامه ریزی مجدد شاخص استراتژیک";
    public const string DELETE_MEASURE = "تایید حذف برنامه ریزی شاخص استراتژیک";
    public const string CREATE_MEASURE_PROGRESS = "تایید پیشرفت شاخص استراتژیک";
    public const string CREATE_MEASURE_EVALUATION = "تایید ارزیابی شاخص استراتژیک";
    public const string CREATE_ACTION_PLAN_CATALOG = "تایید اقدام استراتژیک";
    public const string CREATE_ACTION_PLAN = "تایید برنامه ریزی اقدام استراتژیک";
    public const string CREATE_ACTION_REPLAN = "تایید برنامه ریزی مجدد اقدام استراتژیک";
    public const string DELETE_ACTION_PLAN_CATALOG = "تایید حذف اقدام استراتژیک";
    public const string DELETE_ACTION_PLAN = "تایید حذف برنامه ریزی اقدام استراتژیک";
    public const string CREATE_ACTION_PROGRESS = "تایید پیشرفت اقدام استراتژیک";
    public const string CREATE_ACTION_EVALUATION = "تایید ارزیابی اقدام استراتژیک";
    public const string CREATE_EVALUATION_FEEDBACK = "تایید بازخورد ارزیابی";
    public const string STATUS_DESCRIPTION_PREFIX = "در حال بررسی و تایید";
    public const string STATUS_COMPLETED = "تایید نهایی";
}
