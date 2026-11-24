using SemBackDotNet10.analysisDomain.model.dto.pestel;
using SemBackDotNet10.analysisDomain.model.entity.pestel;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.interf.pestel;

public interface IPestelScoreService
{
    Task<PestelScore> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task CreateNewPestelScoreAsync(PestelScoreDto pestelScoreDto, CancellationToken ct = default);

    Task DeletePestelScoreAsync(long? pestelScoreId, CancellationToken ct = default);

    Task EditPestelScoreAsync(PestelScoreDto pestelScoreDto, CancellationToken ct = default);

    Task<SearchDto> GetAllByPestelGroupIdAsync(long? pestelGroupId, int page, int size, string? orderBy, SortDirection? direction, CancellationToken ct = default);

    Task<List<PestelScoreDto>> GetKeyOpportunityByPestelGroupAsync(string body, CancellationToken ct = default);

    Task<List<PestelScoreDto>> GetKeyThreatByPestelGroupAsync(string body, CancellationToken ct = default);
}
