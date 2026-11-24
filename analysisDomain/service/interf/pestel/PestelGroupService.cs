using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.pestel;
using SemBackDotNet10.analysisDomain.model.entity.pestel;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.interf.pestel;

public interface IPestelGroupService
{
    Task<PestelGroup> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<PestelGroupDto> CreateNewPestelGroupAsync(PestelGroupDto pestelGroup, CancellationToken ct = default);

    Task DeletePestelGroupByIdAsync(long? id, CancellationToken ct = default);

    Task<PestelGroupDto> EditPestelGroupAsync(PestelGroupDto pestelGroup, CancellationToken ct = default);

    Task<SearchDto> FindAllPageAsync(int page, int size, string? orderBy, SortDirection? direction, PestelGroupDto? dto, CancellationToken ct = default);

    Task<List<PestelGroupDto>> FindAllPestelGroupsAsync(long? planId, CancellationToken ct = default);
}
