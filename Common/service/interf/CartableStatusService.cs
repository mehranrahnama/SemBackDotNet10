using Common.controller;
using Common.model.dto;
using Common.repository;
namespace Common.service.interf;
// Host must resolve real roles with the same not-found/authorization behavior as RoleService.
public interface ICartableStatusRoleHost { SemBackDotNet10.SecurityDomain.Model.Entity.Role FindByIdOrElseThrow(long id); }
// Must execute delete-and-create on the same persistence session, atomically.
public interface ICartableStatusWriteScope { void Execute(Action action); }
public interface CartableStatusService:ICartableStatusControllerService
{
    CartableStatusRepository GetRepository();
    long? FindNewStatus(string entityName,string outCome,long? confirmStatus);
    CartableStatusPair FindCartableStatusPair(string entityName,string outCome,long? confirmStatus,string? operationType);
}
