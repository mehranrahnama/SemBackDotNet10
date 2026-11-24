using PageUtils = global::Common.utils.PageUtils;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.analysisDomain.repository.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;
using SemBackDotNet10.Common.exception;


namespace SemBackDotNet10.analysisDomain.service.impl.porter;

public class PorterServiceImpl : IPorterService
{
    private readonly IPorterRepository _porterRepository;
    private readonly IPorterGroupService _porterGroupService;

    public PorterServiceImpl(IPorterRepository porterRepository, IPorterGroupService porterGroupService)
    {
        _porterRepository = porterRepository;
        _porterGroupService = porterGroupService;
    }

    public IPorterRepository GetRepository() => _porterRepository;

    public async Task<Porter> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.Porter.idIsNull");

        return await _porterRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.Porter.idIsNotValid");
    }

    public async Task<PorterDto> CreateNewPorterAsync(PorterDto porterDto, CancellationToken ct = default)
    {
        if (porterDto == null)
            throw new BusinessException("error.PorterDto.isNull");

        var entity = await MapToEntityAsync(porterDto, ct);
        if (entity.Id == 0)
            await _porterRepository.AddAsync(entity, ct);
        else
            await _porterRepository.UpdateAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task UpdatePorterAsync(PorterDto porterDto, CancellationToken ct = default)
    {
        var porter = await FindByIdOrElseThrowAsync(porterDto.Id, ct);

        if (porterDto.PorterGroupDto != null)
        {
            var porterGroup = await _porterGroupService.FindByIdOrElseThrowAsync(porterDto.PorterGroupDto.Id, ct);
            porter.PorterGroup = porterGroup;
            porter.PorterGroupId = porterGroup.Id;
        }

        if (porterDto.Title != null)
            porter.Title = porterDto.Title;

        porter.NoThreat = porterDto.NoThreat ?? string.Empty;
        porter.MiddleThreat = porterDto.MiddleThreat ?? string.Empty;
        porter.SeriousThreat = porterDto.SeriousThreat ?? string.Empty;

        await _porterRepository.UpdateAsync(porter, ct);
    }

    public async Task DeletePorterByIdAsync(long? porterId, CancellationToken ct = default)
    {
        var result = await FindByIdOrElseThrowAsync(porterId, ct);
        await _porterRepository.DeleteAsync(result, ct);
    }

    public async Task<SearchDto> GetAllPorterPageByPorterGroupAsync(int page, int size, string? orderBy, SortDirection? direction, string? body, CancellationToken ct = default)
    {
        var json = PorterJsonHelper.Parse(body);
        var filter = PorterJsonHelper.GetString(json, "filter");
        var porterGroupId = PorterJsonHelper.GetLong(json, "porterGroupId");
        var pageRequest = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = await _porterRepository.FindAllPageAsync(porterGroupId, filter, pageRequest, ct);
        return new SearchDto.Builder()
            .PageSize(size).CurrentPage(page).TotalPages(result.TotalPages)
            .TotalElements(result.TotalElements)
            .Content(result.Items)
            .Build();
    }

    public Task<List<PorterDto>> GetAllPorterByPorterGroupAsync(string? body, CancellationToken ct = default)
    {
        var json = PorterJsonHelper.Parse(body);
        var filter = PorterJsonHelper.GetString(json, "filter");
        var porterGroupId = PorterJsonHelper.GetLong(json, "porterGroupId");
        return _porterRepository.FindAllAsync(porterGroupId, filter, ct);
    }

    private PorterDto MapToDto(Porter porter) =>
        new()
        {
            Id = porter.Id,
            Title = porter.Title,
            PorterGroupDto = _porterGroupService.MapToDto(porter.PorterGroup),
            NoThreat = porter.NoThreat,
            MiddleThreat = porter.MiddleThreat,
            SeriousThreat = porter.SeriousThreat
        };

    private async Task<Porter> MapToEntityAsync(PorterDto porterDto, CancellationToken ct)
    {
        var porterGroup = await _porterGroupService.FindByIdOrElseThrowAsync(porterDto.PorterGroupDto?.Id, ct);
        return new Porter
        {
            Id = porterDto.Id ?? 0,
            Title = porterDto.Title ?? string.Empty,
            PorterGroup = porterGroup,
            PorterGroupId = porterGroup.Id,
            NoThreat = porterDto.NoThreat ?? string.Empty,
            MiddleThreat = porterDto.MiddleThreat ?? string.Empty,
            SeriousThreat = porterDto.SeriousThreat ?? string.Empty
        };
    }
}
