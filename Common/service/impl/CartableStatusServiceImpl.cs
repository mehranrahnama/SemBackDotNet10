using System.Text.Json;
using Common.exception;
using Common.model.dto;
using Common.model.entity;
using Common.repository;
using Common.service.interf;
using Common.utils;
using Common.utils.cartable;
using Common.utils.userRoles;
namespace Common.service.impl;
public sealed class CartableStatusServiceImpl(CartableStatusRepository repository,UserRolesHelpers userRoles,
    string tenantName,ICartableStatusRoleHost roles,ICartableStatusWriteScope writes):CartableStatusService
{
    public CartableStatusRepository GetRepository()=>repository;
    public long? FindNewStatus(string entityName,string outCome,long? confirmStatus)
    {
        var result=repository.FindByEntityNameIgnoreCaseAndOutComeIgnoreCaseAndOldStatus(entityName,outCome,confirmStatus);
        if(result is null) throw new InvalidOperationException($" NOT FOUND: entityName: {entityName} outCome: {outCome} confirmStatus: {confirmStatus} ");
        return result.NewStatus;
    }
    public void CreateCartableStatusLink(List<CartableStatusDto> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        if(items.Count==0) throw new ArgumentException("At least one cartable status is required.",nameof(items));
        writes.Execute(()=>{
            var last=items[^1];
            List<CartableStatus> previous;
            if(last.OutCome=="confirm"&&last.OperationType=="create")
                previous=repository.FindByEntityNameAndCreateOperationTypeAndConfirm(last.EntityName!,last.OperationType);
            else if(last.OutCome=="confirm"&&last.OperationType=="delete")
                previous=repository.FindByEntityNameAnDeleteOperationTypeAndConfirm(last.EntityName!,last.OperationType);
            else if((last.OutCome=="replanConfirm"||last.OutCome=="replanReject")&&last.OperationType=="create")
                previous=repository.FindByEntityNameAndCreateOperationTypeAndReplan(last.EntityName!,last.OperationType,last.OutCome);
            else previous=repository.FindByEntityNameAndOperationTypeAnReject(last.EntityName!,last.OperationType!);
            foreach(var value in previous) repository.Delete(value);
            if(last.OutCome is null) return;
            foreach(var dto in items) {
                var value=new CartableStatus {
                    OperationType=dto.OperationType,EntityName=dto.EntityName,OldStatus=dto.OldStatus,NewStatus=dto.NewStatus,
                    OldStatusMeaning=dto.OldStatusMeaning,NewStatusMeaning=dto.NewStatusMeaning,OutCome=dto.OutCome,
                    FromRole=dto.FromRoleId is long from ? roles.FindByIdOrElseThrow(from):null,
                    ToRole=dto.ToRoleId is long to ? roles.FindByIdOrElseThrow(to):null
                };
                // Deliberate correction: Java used reference equality (==) for this string.
                if(dto.OutCome!="preConfirm"||repository.FindByEntityNameAndOutCome(dto.EntityName!,dto.OutCome) is null)
                    repository.Save(value);
            }
        });
    }
    public List<CartableStatusDto> GetByEntityNameAndOperationTypeAndOutCome(CartableStatusDto dto)=>
        repository.FindListByEntityNameAndOperationTypeAndOutCome(dto.EntityName!,dto.OperationType!,dto.OutCome!);
    // Java intentionally queries only entityName, despite this method's name.
    public List<CartableStatusDto> GetByEntityNameAndOperationType(CartableStatusDto dto)=>repository.FindListByEntityName(dto.EntityName!);
    public List<CartableStatusDto> FindConfirmationLink(string? body)
    {
        using var document=JsonDocument.Parse(body??throw new ArgumentNullException(nameof(body)));
        var root=document.RootElement;
        string? Value(string key)=>!root.TryGetProperty(key,out var value)||value.ValueKind==JsonValueKind.Null?null:value.ToString();
        return repository.FindAllByEntityNameAndOperationTypeAndOutCome(Value("entityName")!,Value("operationType"),Value("outcome")!);
    }
    public CartableStatusPair FindCartableStatusPair(string entityName,string outCome,long? confirmStatus,string? operationType)
    {
        if(string.Equals(tenantName,TenantNameConstants.TENANT_HOLDING,StringComparison.OrdinalIgnoreCase)) {
            bool creation=string.Equals(operationType,CartableStatusConstants.ACTION_CREATE,StringComparison.OrdinalIgnoreCase)
                ||(confirmStatus>=CartableStatusConstants.START_CREATION_CONFIRMATION_STATUS&&confirmStatus<=CartableStatusConstants.FINAL_CREATION_CONFIRMATION_STATUS);
            return new(){OldStatus=confirmStatus,NewStatus=creation?CartableStatusConstants.FINAL_CREATION_CONFIRMATION_STATUS:CartableStatusConstants.FINAL_DELETION_CONFIRMATION_STATUS};
        }
        var names=userRoles.GetUserRoleNames();
        operationType??=CartableStatusUtils.FindCartableStatusOperationType(confirmStatus??throw new ArgumentNullException(nameof(confirmStatus)));
        var pair=userRoles.IsCurrentUserAdmin()
            ?new CartableStatusPair{OldStatus=confirmStatus,NewStatus=CartableStatusUtils.FindNewStatusForAdminByOperationType(outCome,operationType)}
            :repository.FindMaxNewStatusByEntityAndOperationAndOutcomeAndRoles(entityName,operationType,outCome,names,confirmStatus)
                ??throw new BusinessException("کاربر مورد نظر دسترسی لازم برای درخواست را ندارد");
        if(operationType==CartableStatusConstants.ACTION_DELETE&&outCome==CartableStatusConstants.OUTCOME_REJECT
            &&pair.NewStatus==CartableStatusConstants.START_DELETION_CONFIRMATION_STATUS)
            pair.NewStatus=CartableStatusConstants.FINAL_CREATION_CONFIRMATION_STATUS;
        return pair;
    }
}
