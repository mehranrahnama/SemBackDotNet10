// C# migration of utils/cartable/CartableStatusConstants.java

namespace Common.utils.cartable;

public static class CartableStatusConstants
{
    public const string ACTION_CREATE = "create";
    public const string ACTION_DELETE = "delete";
    public const long FINAL_CREATION_CONFIRMATION_STATUS = 20L;
    public const long FINAL_DELETION_CONFIRMATION_STATUS = 50L;
    public const long START_DELETION_CONFIRMATION_STATUS = 30L;
    public const long START_CREATION_CONFIRMATION_STATUS = 0L;
    public const long FIRST_CREATION_CONFIRMATION_STATUS = 1L;
    public const string OUTCOME_REJECT = "reject";
    public const string OUTCOME_CONFIRM = "confirm";
    public const string OUTCOME_REPLAN_CONFIRM = "replanConfirm";
    public const string OUTCOME_REPLAN_REJECT = "replanReject";
}
