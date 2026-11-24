using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;
using SemBackDotNet10.dashboardDomain.model.dto;
using SemBackDotNet10.dashboardDomain.model.entity;

namespace SemBackDotNet10.dashboardDomain.repository;

public interface IDashboardReportAllLevelScoreRepository
{
    Task<HashSet<AllLevelScoreDto>> FindAllLevelScoreAsync(long planId, long month, long fetchType);
}

public class DashboardReportAllLevelScoreRepository : IDashboardReportAllLevelScoreRepository
{
    private readonly SemDbContext _context;

    public DashboardReportAllLevelScoreRepository(SemDbContext context) => _context = context;

    public async Task<HashSet<AllLevelScoreDto>> FindAllLevelScoreAsync(long planId, long month, long fetchType)
    {
        var rows = await _context.Set<DashboardReportAllLevelScore>().AsNoTracking()
            .Where(r => r.PlanId == planId && r.Month == month && r.FetchType == fetchType)
            .Select(r => new AllLevelScoreDto
            {
                PlanId = r.PlanId,
                Month = r.Month,
                LevelId = r.LevelId,
                FetchType = r.FetchType,
                DesLevel = r.DesLevel,
                DesOrganization = r.DesOrganization,
                ScoreActualSelf = r.ScoreActualSelf,
                ScoreEval = r.ScoreEval,
                ObjectCountGreen = r.ObjectCountGreen,
                ObjectCountYellow = r.ObjectCountYellow,
                ObjectCountRed = r.ObjectCountRed,
                ObjectCountGray = r.ObjectCountGray
            })
            .ToListAsync();
        return rows.ToHashSet();
    }
}
