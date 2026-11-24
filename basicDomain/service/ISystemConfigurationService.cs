using global::SemBackDotNet10.Models.Entities;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
namespace SemBackDotNet10.basicDomain.service;

public interface ISystemConfigurationService
{
    SearchDto FindAll(string search, int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction);
    SystemConfigurationDto MapToDto(SystemConfiguration systemConfiguration);
    SystemConfiguration MapToEntity(SystemConfigurationDto systemConfigurationDto);
    SystemConfiguration Edit(SystemConfigurationDto systemConfigurationDto);
    SystemConfiguration FindByIdOrElseThrow(long id);
    SystemConfiguration Create(SystemConfigurationDto systemConfigurationDto);
    void Delete(long id);
}
