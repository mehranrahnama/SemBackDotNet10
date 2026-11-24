using System.Text.Json;
using Common.exception;
using Common.logging;
using Common.utils;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.pestel;
using SemBackDotNet10.analysisDomain.model.entity.pestel;
using SemBackDotNet10.analysisDomain.repository.pestel;
using SemBackDotNet10.analysisDomain.service.interf.pestel;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;


namespace SemBackDotNet10.analysisDomain.service.impl.pestel;

public class PestelServiceImpl : IPestelService
{
    private readonly IPestelGroupService _pestelGroupService;
    private readonly IPestelRepository _pestelRepository;
    private readonly SemLogger _semLogger;

    public PestelServiceImpl(IPestelGroupService pestelGroupService, IPestelRepository pestelRepository, SemLogger semLogger)
    {
        _pestelGroupService = pestelGroupService;
        _pestelRepository = pestelRepository;
        _semLogger = semLogger;
    }

    public async Task<Pestel> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.Pestel.idIsNull");
        return await _pestelRepository.FindByIdAsync(id.Value, ct)
               ?? throw new BusinessException("error.Pestel.idIsNotValid");
    }

    public async Task<PestelDto> GetPestelByIdAsync(long? id, CancellationToken ct = default) =>
        MapToDto(await FindByIdOrElseThrowAsync(id, ct));

    public async Task CreateNewPestelAsync(List<PestelDto> pestelDto, CancellationToken ct = default)
    {
        var transientEntities = new List<Pestel>();
        foreach (var dto in pestelDto)
            transientEntities.Add(await MapToEntityAsync(dto, ct));
        await _pestelRepository.AddRangeAsync(transientEntities, ct);
    }

    public async Task UpdatePestelAsync(List<PestelDto> pestelDto, CancellationToken ct = default)
    {
        var transientEntities = new List<Pestel>();
        foreach (var dto in pestelDto)
        {
            var existingPestel = await FindByIdOrElseThrowAsync(dto.Id, ct);
            if (dto.PestelGroupId != null && dto.PestelGroupId != existingPestel.PestelGroupId)
            {
                var pestelGroup = await _pestelGroupService.FindByIdOrElseThrowAsync(dto.PestelGroupId, ct);
                existingPestel.PestelGroup = pestelGroup;
                existingPestel.PestelGroupId = pestelGroup.Id;
            }
            if (dto.Title != null)
                existingPestel.Title = dto.Title;
            transientEntities.Add(existingPestel);
        }
        await _pestelRepository.UpdateRangeAsync(transientEntities, ct);
    }

    public async Task DeletePestelByIdAsync(long? pestelId, CancellationToken ct = default)
    {
        if (pestelId == null)
            throw new BusinessException("error.Pestel.idIsNull");
        var pestel = await FindByIdOrElseThrowAsync(pestelId, ct);
        try
        {
            await _pestelRepository.DeleteAsync(pestel, ct);
        }
        catch (DbUpdateException e) when (HasConstraintMessage(e))
        {
            throw new BusinessException("امکان حذف به دلیل استفاده عامل محیطی در ماتریس/امتیاز عوامل محیطی وجود ندارد.");
        }
    }

    public async Task<SearchDto> GetAllPestelPageByPestelGroupAsync(int page, int size, string? orderBy, SortDirection? direction, string? filter, long? pestelGroupId, CancellationToken ct = default)
    {
        var pageRequest = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = await _pestelRepository.FindAllPestelPageAsync(pageRequest, filter, pestelGroupId, ct);
        return new SearchDto.Builder()
            .Content(result.Items)
            .TotalElements(result.TotalElements)
            .TotalPages(result.TotalPages)
            .PageSize(size)
            .CurrentPage(page)
            .Build();
    }

    public Task<List<PestelDto>> GetPestelPriorityListByPestelGroupIdAsync(string body, CancellationToken ct = default)
    {
        using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(body) ? "{}" : body);
        var pestelGroupId = ReadLong(doc.RootElement, "pestelGroupId");
        return _pestelRepository.GetPestelPriorityListByPestelGroupIdAsync(pestelGroupId, ct);
    }

    public async Task UpdatePestelPriorityAsync(List<PestelDto> dtoList, CancellationToken ct = default)
    {
        var transientEntities = new List<Pestel>();
        foreach (var dto in dtoList)
        {
            var pestel = await FindByIdOrElseThrowAsync(dto.Id, ct);
            pestel.Impact = dto.Impact;
            pestel.Uncertainty = dto.Uncertainty;
            transientEntities.Add(pestel);
        }
        await _pestelRepository.UpdateRangeAsync(transientEntities, ct);
    }

    public async Task DeletePestelPriorityAsync(List<PestelDto> dtoList, CancellationToken ct = default)
    {
        var transientEntities = new List<Pestel>();
        foreach (var dto in dtoList)
        {
            var pestel = await FindByIdOrElseThrowAsync(dto.Id, ct);
            pestel.Impact = null;
            pestel.Uncertainty = null;
            transientEntities.Add(pestel);
        }
        await _pestelRepository.UpdateRangeAsync(transientEntities, ct);
    }

    private async Task<Pestel> MapToEntityAsync(PestelDto dto, CancellationToken ct)
    {
        var pestelGroup = await _pestelGroupService.FindByIdOrElseThrowAsync(dto.PestelGroupId, ct);
        return new Pestel
        {
            PestelGroup = pestelGroup,
            PestelGroupId = pestelGroup.Id,
            Title = dto.Title ?? string.Empty
        };
    }

    private static PestelDto MapToDto(Pestel pestel) => new()
    {
        Id = pestel.Id,
        PestelGroupId = pestel.PestelGroupId,
        Title = pestel.Title
    };

    private static bool HasConstraintMessage(Exception e)
    {
        for (var ex = (Exception?)e; ex != null; ex = ex.InnerException)
        {
            if (ex.Message.Contains("constraint", StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private static long? ReadLong(JsonElement root, string key)
    {
        if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty(key, out var value))
            return null;
        return value.ValueKind switch
        {
            JsonValueKind.Number => value.GetInt64(),
            JsonValueKind.String when long.TryParse(value.GetString(), out var parsed) => parsed,
            _ => null
        };
    }
}
