using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.resource;
using SemBackDotNet10.analysisDomain.model.entity.resource;


namespace SemBackDotNet10.analysisDomain.service.interf.resource;

public interface IResourceService
{
    Task<ResourceDto> CreateNewResourceAsync(ResourceDto resourceDto, CancellationToken ct = default);

    Task<Resource> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<SearchDto> GetAllResourcePageByLevelIdAsync(int page, int size, string? orderBy, SortDirection? direction, string body, CancellationToken ct = default);

    Task<List<ResourceDto>> GetAllResourceByLevelIdAsync(string body, CancellationToken ct = default);

    Task UpdateResourceAsync(ResourceDto resourceDto, CancellationToken ct = default);

    Task DeleteResourceByIdAsync(long? resourceId, CancellationToken ct = default);

    Task EditResourceCompetencyAsync(ResourceDto resourceDto, CancellationToken ct = default);

    Task<SearchDto> GetKeyResourceByPlanIdAsync(int page, int size, string? orderBy, SortDirection? direction, string body, CancellationToken ct = default);

    Task<List<ResourceDto>> GetKeyResourceByPlanIdListAsync(string body, CancellationToken ct = default);
}
