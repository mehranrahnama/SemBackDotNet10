using System.Linq;
using Common.utils;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.Base.model.dto;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public SearchDto findAllByActionPlanCatalogId(long? actionPlanId, int page,
        int size, string? orderBy, SortDirection direction)
        => FindCatalogPageResponse(actionPlanId, page, size, orderBy, direction, false);

    public SearchDto findAllByActionPlanCatalogId_V2(long? actionPlanId, int page,
        int size, string? orderBy, SortDirection direction)
        => FindCatalogPageResponse(actionPlanId, page, size, orderBy, direction, true);

    private SearchDto FindCatalogPageResponse(long? catalogId, int page,
        int size, string? orderBy, SortDirection direction, bool deleted)
    {
        if (catalogId is null) throw new BusinessException("error.ActionActPlan.getAll");
        var request = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = _actionActPlanRepository.FindCatalogPage(catalogId.Value, request, deleted);
        return new SearchDto.Builder().content(result.Content.ToList())
            .totalElements(result.TotalElements).totalPages(checked((int)result.TotalPages))
            .currentPage(request.Page).pageSize(request.Size).build();
    }
}
