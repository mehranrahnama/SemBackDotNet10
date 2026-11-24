using System.Text.Json;
using Common.utils;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public SearchDto relatedValueGrid(string? body, int page, int size, string? orderBy, SortDirection direction)
    {
        try
        {
            using var json = JsonDocument.Parse(body!);
            long? id = json.RootElement.TryGetProperty("actId", out var value) && value.ValueKind != JsonValueKind.Null
                ? value.GetInt64() : null;
            var request = PageUtils.GetInstance(page, size, orderBy, direction);
            var result = _actionActPlanRepository.FindRelatedValues(id, request);
            return new SearchDto.Builder().content(result.Content.ToList())
                .totalElements(result.TotalElements).totalPages(checked((int)result.TotalPages))
                .currentPage(page).pageSize(size).build();
        }
        catch (JsonException error)
        {
            throw new BusinessException(error.Message);
        }
    }
}
