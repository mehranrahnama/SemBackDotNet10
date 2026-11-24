// C# migration of utils/measure/MeasureActPlanConstants.java

namespace Common.utils.measure;

public static class MeasureActPlanConstants
{
    public const int DESIRED_METHOD_ASCENDING = 1;
    public const int DESIRED_METHOD_DESCENDING = 2;
    public const int DESIRED_METHOD_KEEP_STATE = 3;
    public const int DESIRED_METHOD_STRONGLY_ASCENDING = 4;
    public const int DESIRED_METHOD_STRONGLY_DESCENDING = 5;
    public const int DESIRED_METHOD_ASCENDING_WITHOUT_VALIDATION = 6;
    public const int DESIRED_METHOD_DESCENDING_WITHOUT_VALIDATION = 7;
    public const int MEASURE_PERIOD_MONTHLY = 2;
    public const int MEASURE_PERIOD_SEASONAL = 3;
    public const int MEASURE_PERIOD_SIX_MONTH = 4;
    public const int MEASURE_PERIOD_YEARLY = 5;
    public const int MEASURE_PERIOD_CUSTOM = 6;
    public const int MONTHLY_MULTIPLE_NUMBER = 1;
    public const int SEASONAL_MULTIPLE_NUMBER = 3;
    public const int SIX_MONTH_MULTIPLE_NUMBER = 6;
    public const int YEARLY_MULTIPLE_NUMBER = 12;
    public const int CUSTOM_MULTIPLE_NUMBER = 13;
    public const int NUMBER_OF_MONTHS_IN_YEAR = 12;
    public const int MEASURE_EVAL_PERIODIC = 1;
    public const int MEASURE_EVAL_CUMULATIVE = 2;
    public const int MEASURE_EVAL_AVERAGE = 3;
    public const int MEASURE_EVAL_OTHER = 4;
}
