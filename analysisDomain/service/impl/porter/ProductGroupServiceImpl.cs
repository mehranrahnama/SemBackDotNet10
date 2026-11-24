using PageUtils = global::Common.utils.PageUtils;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using Common.exception;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.analysisDomain.repository.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;
using SemBackDotNet10.Common.exception;


namespace SemBackDotNet10.analysisDomain.service.impl.porter;

public class ProductGroupServiceImpl : IProductGroupService
{
    private readonly IProductGroupRepository _productGroupRepository;
    private readonly global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository _planPeriodService;

    public ProductGroupServiceImpl(IProductGroupRepository productGroupRepository, global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository planPeriodService)
    {
        _productGroupRepository = productGroupRepository;
        _planPeriodService = planPeriodService;
    }

    public IProductGroupRepository GetRepository() => _productGroupRepository;

    public async Task<ProductGroup> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.ProductGroup.idIsNull");

        return await _productGroupRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.ProductGroup.idIsNotValid");
    }

    public async Task<ProductGroupDto> CreateProductGroupAsync(ProductGroupDto productGroupDto, CancellationToken ct = default)
    {
        if (productGroupDto == null)
            throw new BusinessException("error.PorterGroup.isNull");

        var entity = await MapToEntityAsync(productGroupDto, ct);
        if (entity.Id == 0)
            await _productGroupRepository.AddAsync(entity, ct);
        else
            await _productGroupRepository.UpdateAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task<SearchDto> FindAllPageAsync(int page, int size, string? orderBy, SortDirection? direction, string? body, CancellationToken ct = default)
    {
        var json = PorterJsonHelper.Parse(body);
        var filter = PorterJsonHelper.GetString(json, "filter");
        var planId = PorterJsonHelper.OptLong(json, "planId");
        var pageRequest = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = await _productGroupRepository.FindAllPageAsync(filter, planId, pageRequest, ct);
        return new SearchDto.Builder()
            .PageSize(size).CurrentPage(page).TotalPages(result.TotalPages)
            .TotalElements(result.TotalElements)
            .Content(result.Items)
            .Build();
    }

    public Task<List<ProductGroupDto>> FindAllProductGroupsAsync(long? planId, CancellationToken ct = default) =>
        _productGroupRepository.FindAllByPlanPeriodPlanIdAsync(planId, ct);

    public async Task<ProductGroupDto> EditProductGroupAsync(ProductGroupDto productGroupDto, CancellationToken ct = default)
    {
        var result = await FindByIdOrElseThrowAsync(productGroupDto.Id, ct);
        result.Title = productGroupDto.Title ?? string.Empty;
        await _productGroupRepository.UpdateAsync(result, ct);
        return MapToDto(result);
    }

    public async Task DeleteProductGroupAsync(long? id, CancellationToken ct = default)
    {
        var result = await FindByIdOrElseThrowAsync(id, ct);
        await _productGroupRepository.DeleteAsync(result, ct);
    }

    public async Task<ProductGroup> MapToEntityAsync(ProductGroupDto productGroupDto, CancellationToken ct = default)
    {
        var planPeriod = await _planPeriodService.FindPlanPeriodByPlanIdAsync(
            productGroupDto.PlanId ?? throw new BusinessException("error.planPeriod.notFound"), ct);
        return new ProductGroup
        {
            Id = productGroupDto.Id ?? 0,
            Title = productGroupDto.Title ?? string.Empty,
            PlanPeriod = planPeriod,
            PlanId = planPeriod.PlanId
        };
    }

    public ProductGroupDto MapToDto(ProductGroup productGroup) =>
        new()
        {
            Id = productGroup.Id,
            Title = productGroup.Title,
            PlanId = productGroup.PlanId
        };
}
