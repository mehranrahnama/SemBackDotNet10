using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using System.Globalization;
using Common.utils;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.Data;
using SemBackDotNet10.webserviceDomain.domain.dto;
using SemBackDotNet10.webserviceDomain.repository;
using SemBackDotNet10.webserviceDomain.service.interf;
using IActionPlanService = SemBackDotNet10.actionDomain.service.interf.IActionPlanService;

namespace SemBackDotNet10.webserviceDomain.service.impl;

public class WebServicesServiceImpl : IWebServicesService
{
    private readonly SemDbContext _context;
    private readonly IActionPlanService _actionPlanService;
    private readonly IWebServiceRepository _webServiceRepository;

    public WebServicesServiceImpl(
        SemDbContext context,
        IActionPlanService actionPlanService,
        IWebServiceRepository webServiceRepository)
    {
        _context = context;
        _actionPlanService = actionPlanService;
        _webServiceRepository = webServiceRepository;
    }

    public async Task<SearchDto> FindAllStrategyServicesAsync(int page, int size, string? orderBy, SortDirection? direction, CancellationToken ct = default)
    {
        var pageable = PageUtils.GetInstance(page, size, orderBy, direction);

        // PlanId must be mapped on StrategyObject (shadow property or real column).
        var query = _context.StrategyObjects
            .Where(s => EF.Property<long?>(s, "PlanId") == 140301L && !s.IsDeleted)
            .Select(s => new StrategyObjectWebServiceDto { Id = s.Id, Name = s.DesStrategyObjectF });

        var total = await query.LongCountAsync(ct);
        var content = await query.Apply(pageable).ToListAsync(ct);
        var totalPages = (int)Math.Ceiling(total / (double)pageable.Size);

        return new SearchDto.Builder()
            .Content(content)
            .TotalElements(total)
            .TotalPages(totalPages)
            .CurrentPage(page)
            .PageSize(pageable.Size)
            .Build();
    }

    public Task<List<ActionPlanWebServiceDto>> FindAllActionPlansAsync(CancellationToken ct = default) =>
        Task.FromResult(_actionPlanService.findActionPlanForWebService(140501L));

    public Task<double> GetObjectFunctionalityAsync(CancellationToken ct = default)
    {
        var (planId, month) = CurrentPlanAndMonth();
        return _webServiceRepository.ObjectFunctionalityAsync(planId, month, ct);
    }

    public Task<double> GetActionPCNScoreAsync(CancellationToken ct = default)
    {
        var (planId, month) = CurrentPlanAndMonth();
        return _webServiceRepository.ActionPCNScoreAsync(planId, month, ct);
    }

    // Equivalent of DateUtils.currentDateTimePersianInString("yyyyMMdd_HHmmss", "en") split into plan id and month.
    private static (long PlanId, long Month) CurrentPlanAndMonth()
    {
        var calendar = new PersianCalendar();
        var now = DateTime.Now;
        var date = $"{calendar.GetYear(now):0000}{calendar.GetMonth(now):00}{calendar.GetDayOfMonth(now):00}";
        var planId = long.Parse(date.Substring(0, 4) + "01", CultureInfo.InvariantCulture);
        var month = long.Parse(date.Substring(4, 2), CultureInfo.InvariantCulture);
        return (planId, month);
    }
}
