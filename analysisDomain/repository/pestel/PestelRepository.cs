using global::SemBackDotNet10.Common.Pagination;
using Common.utils;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.pestel;
using SemBackDotNet10.analysisDomain.model.entity.pestel;

using SemBackDotNet10.Data;
using System.Linq.Dynamic.Core;

namespace SemBackDotNet10.analysisDomain.repository.pestel;

public interface IPestelRepository : IBaseRepository<Pestel>
{
    Task<global::SemBackDotNet10.Common.Pagination.PagedResult<PestelDto>> FindAllPestelPageAsync(PageRequest pageRequest, string? filter, long? pestelGroupId, CancellationToken ct = default);

    Task<List<PestelDto>> GetPestelPriorityListByPestelGroupIdAsync(long? pestelGroupId, CancellationToken ct = default);
}

public class PestelRepository : BaseRepository<Pestel>, IPestelRepository
{
    public PestelRepository(SemDbContext context) : base(context) { }

    public async Task<global::SemBackDotNet10.Common.Pagination.PagedResult<PestelDto>> FindAllPestelPageAsync(PageRequest pageRequest, string? filter, long? pestelGroupId, CancellationToken ct = default)
    {
        var query = Query
            .Where(p => filter == null || p.Title.Contains(filter))
            .Where(p => pestelGroupId == null || p.PestelGroupId == pestelGroupId)
            .OrderBy(p => p.Id)
            .Select(p => new PestelDto
            {
                Id = p.Id,
                PestelGroupId = p.PestelGroupId,
                Title = p.Title
            });

        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageRequest).ToListAsync(ct);
        return new global::SemBackDotNet10.Common.Pagination.PagedResult<PestelDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public Task<List<PestelDto>> GetPestelPriorityListByPestelGroupIdAsync(long? pestelGroupId, CancellationToken ct = default) =>
        Query
            .Where(p => p.PestelGroupId == pestelGroupId && p.Impact != null && p.Uncertainty != null)
            .Select(p => new PestelDto
            {
                Id = p.Id,
                PestelGroupId = p.PestelGroupId,
                PestelGroupTitle = p.PestelGroup.Title,
                Title = p.Title,
                Impact = p.Impact,
                Uncertainty = p.Uncertainty
            })
            .ToListAsync(ct);
}
