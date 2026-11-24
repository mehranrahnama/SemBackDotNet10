using Common.model.dto;
using Common.model.entity;
using Common.repository;
namespace Common.service.interf;
// External BaseService CRUD remains an integration responsibility.
public interface IDesService
{
    IDesRepository GetRepository();
    List<DesDto> GetAllActionTypes();
    List<DesDto> GetAllActionPriorities();
    List<DesDto> GetAllActionMeasureRepositoryStatus();
    List<DesDto> GetAllCertificate();
    Des FindByIdOrElseThrow(long? id);
}
