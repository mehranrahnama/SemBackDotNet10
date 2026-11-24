using Common.exception;
using Common.utils;
using Microsoft.EntityFrameworkCore;
using PositionResponse = SemBackDotNet10.basicDomain.Model.Dto.PositionDto;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.service.impl;

public sealed class PositionEndpointService(SemDbContext db)
{
    public async Task<ServicePage<PositionResponse>> FindAsync(string? filter, int page, int size, string? orderBy,
        SemBackDotNet10.Common.enums.SortDirection direction, CancellationToken ct)
    {
        var request = PageUtils.GetInstance(page, size, orderBy ?? "id", direction);
        var query = db.Set<SemBackDotNet10.basicDomain.Model.Entity.Position>().AsNoTracking()
            .Where(position => filter == null || position.DesPost!.Contains(filter))
            .Select(position => new PositionResponse { Id = position.Id, CodPost = position.CodPost, DesPost = position.DesPost });
        var total = await query.LongCountAsync(ct);
        var rows = await PageUtils.Apply(query, request).ToListAsync(ct);
        return new(rows, total, request.Page, request.Size);
    }

    public async Task<PositionResponse> CreateAsync(PositionResponse dto, CancellationToken ct)
    {
        var entity = new SemBackDotNet10.basicDomain.Model.Entity.Position { DesPost = dto.DesPost, CodPost = dto.CodPost, FactoryId = 1 };
        db.Add(entity);
        await db.SaveChangesAsync(ct);
        return new PositionResponse { Id = entity.Id, CodPost = entity.CodPost, DesPost = entity.DesPost };
    }

    public async Task<PositionResponse> EditAsync(PositionResponse dto, CancellationToken ct)
    {
        var entity = await FindByIdAsync(dto.Id, ct);
        entity.DesPost = dto.DesPost;
        entity.CodPost = dto.CodPost;
        await db.SaveChangesAsync(ct);
        return new PositionResponse { Id = entity.Id, CodPost = entity.CodPost, DesPost = entity.DesPost };
    }

    public async Task DeleteAsync(long id, CancellationToken ct)
    {
        var entity = await FindByIdAsync(id, ct);
        db.Remove(entity);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 547 })
        { throw new BusinessException("error.position.assigned"); }
    }

    private async Task<SemBackDotNet10.basicDomain.Model.Entity.Position> FindByIdAsync(long? id, CancellationToken ct)
        => id is long key ? await db.Set<SemBackDotNet10.basicDomain.Model.Entity.Position>().FindAsync([key], ct)
            ?? throw new BusinessException("error.position.idIsNotValid") : throw new BusinessException("error.position.idIsNotValid");
}
