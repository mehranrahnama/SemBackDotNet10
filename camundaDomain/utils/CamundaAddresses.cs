// Converted from: camundaDomain/utils/CamundaAddresses.java
// جایگیرهای %s جاوا به {0} دات‌نت تبدیل شد (string.Format / uri.formatted معادل string.Format است).

namespace SemBackDotNet10.camundaDomain.utils;

public static class CamundaAddresses
{
    public const string GET_TASK_LIST = "/task";
    public const string CREATE_SINGLE_TASK = "/task/create";
    public const string DELETE_TASK_BY_ID = "/task/{0}";
    public const string GET_TASK_BY_ID = "/task/{0}";
    public const string UPDATE_TASK_BY_ID = "/task/{0}";
    public const string CHANGE_ASSIGNEE_BY_CLAIM = "/task/{0}/claim";
    public const string CHANGE_ASSIGNEE_BY_ASSIGNEE = "/task/{0}/assignee";
    public const string GET_TOTAL_TASK_COUNT = "/task/count";
    public const string DELEGATE_TASK_TO_OTHER = "/task/{0}/delegate";
    public const string REMOVE_CLAIM_FROM_USER = "/task/{0}/unclaim";
    public const string COMPLETE_TASK = "/task/{0}/complete";
    public const string GET_TASK_VARIABLES = "/task/{0}/variables";
    public const string UPDATE_TASK_VARIABLES = "/task/{0}/variables";
    public const string START_PROCESS_INSTANCE = "/process-definition/key/{0}/start";
    public const string UPDATE_PROCESS_VARIABLES = "/process-instance/{0}/variables";
    public const string GET_CANDIDATE_USER_TASKS = "/task?candidateUser={0}";
    public const string GET_PROCESS_DEFINITION_LIST_BY_NAME = "/process-definition";
    public const string DELETE_PROCESS_INSTANCE_BY_ID = "/process-instance/{0}?failIfNotExists=false";
    public const string DELETE_PROCESS_DEFINITION_BY_KEY = "/process-definition/key/{0}?cascade=true";
    public const string DELETE_PROCESS_DEFINITION_BY_ID = "/process-definition/{0}?cascade=true";
}
