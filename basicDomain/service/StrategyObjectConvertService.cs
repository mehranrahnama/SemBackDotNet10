using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SemBackDotNet10.service.impl;

public sealed class ConversionPlaningLevel { public long Id { get; set; } public string? DesLevel { get; set; } public string? DesThreeChar { get; set; } }
public sealed record ConversionPlaningLevelDto(long Id, string? DesLevel, string? DesThreeChar);
public sealed class PlanPeriodRef { public long PlanId { get; set; } }
public sealed class ConversionPerspective { public long Id { get; set; } public string? DesperspectF { get; set; } public string? DesperspectE { get; set; } public PlanPeriodRef PlanPeriod { get; set; } = new(); }
public sealed class ConversionStrategyObject { public long? Id { get; set; } public string? Code { get; set; } public string? Title { get; set; } public double? Weight { get; set; } public ConversionPlaningLevel PlaningLevel { get; set; } = new(); public PlanPeriodRef PlanPeriod { get; set; } = new(); public ConversionPerspective Perspective { get; set; } = new(); }
public sealed class ConversionStrategyObjectDto { public long LevelId { get; set; } public long PlanId { get; set; } public long PerspectId { get; set; } public string? Title { get; set; } public string? Code { get; set; } public double? Weight { get; set; } }
public interface IStrategyObjectRepository
{
    Task<IReadOnlyList<ConversionStrategyObject>> FindAllStrategyObjectByLevelIdAsync(long levelId, long planId, CancellationToken ct = default);
    Task<IReadOnlyList<ConversionStrategyObject>> FindByLevelTitlePlanPerspectiveAsync(long levelId, string? title, long planId, long perspectiveId, CancellationToken ct = default);
}
public interface IPlaningLevelRepository { Task<ConversionPlaningLevel?> FindByDesThreeCharAsync(string code, CancellationToken ct = default); }
public interface IPlanPeriodLookupRepository { Task<PlanPeriodRef?> FindByPlanIdAsync(long planId, CancellationToken ct = default); }
public interface IPerspectiveRepository { Task<ConversionPerspective?> FindByDescriptionAndPlanAndLevelAsync(string? description, long planId, long levelId, CancellationToken ct = default); }
public interface IPlaningLevelService { ConversionPlaningLevelDto MapToDto(ConversionPlaningLevel level); }
public interface IStrategyObjectService { Task CreateAsync(ConversionStrategyObjectDto dto, CancellationToken ct = default); }
public interface IAttachmentService { Task<string> CreateJsonFileAsync(string json, string entityName, CancellationToken ct = default); }

/// <summary>Exports strategy objects as JSON and imports them by resolving level, plan period and perspective.</summary>
/// <remarks>Uses the four repository abstractions, IPlaningLevelService, IStrategyObjectService, IAttachmentService, and a storage directory.</remarks>
public sealed class StrategyObjectConvertService(
    IStrategyObjectRepository strategyObjectRepository, IPlaningLevelRepository planingLevelRepository,
    IPlanPeriodLookupRepository planPeriodRepository, IPerspectiveRepository perspectiveRepository,
    IAttachmentService attachmentService, IStrategyObjectService strategyObjectService,
    IPlaningLevelService planingLevelService, string storageLocation, IServiceLogger? logger = null)
{
    private readonly IServiceLogger _logger = logger ?? new NullServiceLogger();
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

    public async Task<string> ConvertAllDataToJsonAsync(long levelId, long planId, CancellationToken ct = default)
    {
        var records = await strategyObjectRepository.FindAllStrategyObjectByLevelIdAsync(levelId, planId, ct);
        var output = records.Select(item => new Dictionary<string, object?> {
            ["planingLevel"] = planingLevelService.MapToDto(item.PlaningLevel), ["levelId"] = item.PlaningLevel.Id,
            ["desLevel"] = item.PlaningLevel.DesLevel, ["code"] = item.Code, ["title"] = item.Title,
            ["weight"] = item.Weight, ["planId"] = item.PlanPeriod.PlanId, ["perspectiveId"] = item.Perspective.Id,
            ["desPerspectiveF"] = item.Perspective.DesperspectF, ["desPerspectiveE"] = item.Perspective.DesperspectE
        }).ToArray();
        return JsonSerializer.Serialize(output, JsonOptions);
    }
    public async Task<string> CreateJsonFileAsync(long levelId, long planId, CancellationToken ct = default)
    {
        var json = await ConvertAllDataToJsonAsync(levelId, planId, ct);
        return await attachmentService.CreateJsonFileAsync(json, "strategyObjectJson", ct);
    }
    public async Task ImportJsonFileAsync(string jsonFileName, CancellationToken ct = default)
    {
        if (Path.IsPathRooted(jsonFileName)) throw new ArgumentException("Only a file name relative to storageLocation is accepted.", nameof(jsonFileName));
        var root = Path.GetFullPath(storageLocation); var path = Path.GetFullPath(Path.Combine(root, jsonFileName));
        if (!path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.Ordinal) && !string.Equals(path, root, StringComparison.Ordinal)) throw new ArgumentException("File path escapes storage directory.", nameof(jsonFileName));
        var json = await File.ReadAllTextAsync(path, ct);
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind != JsonValueKind.Array) throw new ServiceException("Import JSON must be an array.");
        foreach (var item in doc.RootElement.EnumerateArray())
        {
            var dto = await CreateStrategyObjectDtoAsync(item, ct);
            if (dto is null) continue;
            var existing = await strategyObjectRepository.FindByLevelTitlePlanPerspectiveAsync(dto.LevelId, dto.Title, dto.PlanId, dto.PerspectId, ct);
            // Source behavior checks for no match; an empty result is the insert condition.
            if (existing.Count == 0) await strategyObjectService.CreateAsync(dto, ct);
        }
    }
    private async Task<ConversionStrategyObjectDto?> CreateStrategyObjectDtoAsync(JsonElement item, CancellationToken ct)
    {
        if (!item.TryGetProperty("planingLevel", out var levelJson)) throw new ServiceException("planingLevel is required.");
        var code = GetString(levelJson, "desThreeChar");
        if (string.IsNullOrWhiteSpace(code)) throw new ServiceException("کد سه حرفی سطح برنامه ریزی مشخص نشده است.");
        var level = await planingLevelRepository.FindByDesThreeCharAsync(code, ct) ?? throw new ServiceException("سطح برنامه ریزی " + GetString(levelJson, "desLevel") + " یافت نشد.");
        if (!item.TryGetProperty("planId", out var planElement) || !planElement.TryGetInt64(out var planId)) throw new ServiceException("Invalid planId.");
        var period = await planPeriodRepository.FindByPlanIdAsync(planId, ct) ?? throw new ServiceException("دوره برنامه ریزی" + planId + " یافت نشد. ");
        var perspectiveDescription = GetString(item, "desPerspectiveF");
        var perspective = await perspectiveRepository.FindByDescriptionAndPlanAndLevelAsync(perspectiveDescription, period.PlanId, level.Id, ct)
            ?? throw new ServiceException("منظر یا مضمون" + (perspectiveDescription ?? "") + " یافت نشد.");
        return new ConversionStrategyObjectDto { LevelId = level.Id, PlanId = period.PlanId, PerspectId = perspective.Id,
            Title = GetString(item, "title"), Code = GetString(item, "code"), Weight = GetDouble(item, "weight") };
    }
    private static string? GetString(JsonElement element, string name) => element.TryGetProperty(name, out var p) && p.ValueKind != JsonValueKind.Null ? p.GetString() : null;
    private static double? GetDouble(JsonElement element, string name) => element.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.Number ? p.GetDouble() : null;
}
