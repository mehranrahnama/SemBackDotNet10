// C# migration of utils/CartableStatusUtils.java

namespace Common.utils;

public static class CartableStatusUtils
{
 public static long FindNewStatusForAdminByOperationType(string outCome,string operationType) {
  if(operationType==cartable.CartableStatusConstants.ACTION_CREATE) return outCome==cartable.CartableStatusConstants.OUTCOME_CONFIRM || outCome==cartable.CartableStatusConstants.OUTCOME_REPLAN_CONFIRM ? cartable.CartableStatusConstants.FINAL_CREATION_CONFIRMATION_STATUS : cartable.CartableStatusConstants.START_CREATION_CONFIRMATION_STATUS;
  if(operationType==cartable.CartableStatusConstants.ACTION_DELETE) return outCome==cartable.CartableStatusConstants.OUTCOME_CONFIRM ? cartable.CartableStatusConstants.FINAL_DELETION_CONFIRMATION_STATUS : cartable.CartableStatusConstants.START_DELETION_CONFIRMATION_STATUS;
  throw new global::SemBackDotNet10.Common.exception.BusinessException("مقدار وضعیت تسک نامعتبر است.");
 }
 public static string FindCartableStatusOperationType(long status) {
  if(status>=cartable.CartableStatusConstants.START_CREATION_CONFIRMATION_STATUS && status<=cartable.CartableStatusConstants.FINAL_CREATION_CONFIRMATION_STATUS) return cartable.CartableStatusConstants.ACTION_CREATE;
  if(status>=cartable.CartableStatusConstants.START_DELETION_CONFIRMATION_STATUS && status<=cartable.CartableStatusConstants.FINAL_DELETION_CONFIRMATION_STATUS) return cartable.CartableStatusConstants.ACTION_DELETE;
  throw new global::SemBackDotNet10.Common.exception.BusinessException("مقدار وضعیت تسک نامعتبر است.");
 }
}
