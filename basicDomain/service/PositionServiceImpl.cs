using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SemBackDotNet10.service.impl;

// Shared lightweight contracts and models used by the service implementations in this package.
public interface IServiceLogger { void Info(string message); void Error(string message, Exception? exception = null); }
public sealed class NullServiceLogger : IServiceLogger { public void Info(string message) { } public void Error(string message, Exception? exception = null) { } }
public sealed class ServiceException(string message) : Exception(message);
public sealed record PageResult<T>(IReadOnlyList<T> Items, long TotalElements, int TotalPages);
public sealed record SearchResult<T>(IReadOnlyList<T> Content, long TotalElements, int TotalPages, int CurrentPage, int PageSize);
public enum SortDirection { Ascending, Descending }

public sealed class Position { public long? Id { get; set; } public string? CodPost { get; set; } public string? DesPost { get; set; } public int FactoryId { get; set; } }
public sealed record PositionDto(long? Id, string? CodPost, string? DesPost);
public interface IPositionRepository
{
    Task<PageResult<Position>> FindAllAsync(int page, int size, string orderBy, SortDirection direction, CancellationToken cancellationToken = default);
    Task<PageResult<PositionDto>> SearchByDescriptionAsync(string? keyword, int page, int size, string orderBy, SortDirection direction, CancellationToken cancellationToken = default);
    Task<Position?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Position> SaveAsync(Position position, CancellationToken cancellationToken = default);
    Task DeleteAsync(Position position, CancellationToken cancellationToken = default);
}

/// <summary>CRUD and paged search for positions; persistence and referential-integrity handling belong to IPositionRepository.</summary>
public sealed class PositionServiceImpl(IPositionRepository positionRepository, IServiceLogger? logger = null)
{
    private readonly IServiceLogger _logger = logger ?? new NullServiceLogger();
    public IPositionRepository Repository => positionRepository;

    public async Task<SearchResult<PositionDto>> FindAllAsync(int page, int size, string orderBy = "id", SortDirection direction = SortDirection.Ascending, CancellationToken ct = default)
    {
        ValidatePage(page, size); _logger.Info("Find all positions");
        var result = await positionRepository.FindAllAsync(page, size, orderBy, direction, ct);
        return new(result.Items.Select(MapToDto).ToArray(), result.TotalElements, result.TotalPages, page, size);
    }
    public async Task<SearchResult<PositionDto>> FindPositionsAsync(string? filter, int page, int size, string orderBy = "desPost", SortDirection direction = SortDirection.Ascending, CancellationToken ct = default)
    {
        ValidatePage(page, size);
        var keyword = filter;
        if (!string.IsNullOrWhiteSpace(filter) && filter.TrimStart().StartsWith('{'))
        {
            try { using var doc = System.Text.Json.JsonDocument.Parse(filter); keyword = doc.RootElement.TryGetProperty("filter", out var value) ? value.GetString() : null; }
            catch (System.Text.Json.JsonException ex) { throw new ServiceException($"Invalid position filter JSON: {ex.Message}"); }
        }
        var result = await positionRepository.SearchByDescriptionAsync(keyword, page, size, orderBy, direction, ct);
        return new(result.Items, result.TotalElements, result.TotalPages, page, size);
    }
    public static PositionDto MapToDto(Position p) => new(p.Id, p.CodPost, p.DesPost);
    public async Task<Position> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id is null) throw new ServiceException("error.position.idIsNotValid");
        return await positionRepository.FindByIdAsync(id.Value, ct) ?? throw new ServiceException("error.position.idIsNotValid");
    }
    public async Task HardDeletePositionByIdAsync(long id, CancellationToken ct = default)
    {
        var existing = await FindByIdOrElseThrowAsync(id, ct);
        try { await positionRepository.DeleteAsync(existing, ct); }
        catch (Exception ex) when (ex is not OperationCanceledException) { throw new ServiceException("error.position.assigned"); }
    }
    public async Task<PositionDto> EditPositionByIdAsync(PositionDto dto, CancellationToken ct = default)
    {
        var entity = await FindByIdOrElseThrowAsync(dto.Id, ct);
        entity.DesPost = dto.DesPost ?? entity.DesPost; entity.CodPost = dto.CodPost ?? entity.CodPost;
        return MapToDto(await positionRepository.SaveAsync(entity, ct));
    }
    public async Task<PositionDto> CreateNewPositionAsync(PositionDto dto, CancellationToken ct = default)
    {
        var entity = new Position { CodPost = dto.CodPost, DesPost = dto.DesPost, FactoryId = 1 };
        return MapToDto(await positionRepository.SaveAsync(entity, ct));
    }
    private static void ValidatePage(int page, int size) { if (page < 0 || size <= 0) throw new ArgumentOutOfRangeException(nameof(page), "Page must be non-negative and size must be positive."); }
}
