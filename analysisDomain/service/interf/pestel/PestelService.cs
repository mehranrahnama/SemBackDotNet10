using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.pestel;
using SemBackDotNet10.analysisDomain.model.entity.pestel;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.interf.pestel;

public interface IPestelService
{
    Task<Pestel> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<PestelDto> GetPestelByIdAsync(long? id, CancellationToken ct = default);

    Task CreateNewPestelAsync(List<PestelDto> pestelDto, CancellationToken ct = default);

    Task UpdatePestelAsync(List<PestelDto> pestelDto, CancellationToken ct = default);

    Task DeletePestelByIdAsync(long? pestelId, CancellationToken ct = default);

    Task<SearchDto> GetAllPestelPageByPestelGroupAsync(int page, int size, string? orderBy, SortDirection? direction, string? filter, long? pestelGroupId, CancellationToken ct = default);

    Task<List<PestelDto>> GetPestelPriorityListByPestelGroupIdAsync(string body, CancellationToken ct = default);

    Task UpdatePestelPriorityAsync(List<PestelDto> dtoList, CancellationToken ct = default);

    Task DeletePestelPriorityAsync(List<PestelDto> dtoList, CancellationToken ct = default);
}
