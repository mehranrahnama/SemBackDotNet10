using Common.utils;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Repository;

public interface IControlPanelRepository
{
    PageResult<ControlPanelDto> FindAllControlPanelByEntitytype(int entitytype, PageRequest pageable, string? filter);

    ControlPanel? FindByEntityType(int entitytype);

    List<ControlPanel> FindByEntityTypeAndTashilgarIdAndLevelId(int entitytype, long tashilgarId, long levelId);

    List<ControlPanel> FindByEntityType2AndTashilgarIdAndLevelId(int entitytype, long tashilgarId, long levelId);

    ControlPanel Save(ControlPanel entity);

    void Delete(ControlPanel entity);

    ControlPanel? FindById(long id);
}

public class ControlPanelRepository : SecurityRepositoryBase<ControlPanel>, IControlPanelRepository
{
    public ControlPanelRepository(AppDbContext context) : base(context) { }

    public PageResult<ControlPanelDto> FindAllControlPanelByEntitytype(int entitytype, PageRequest pageable, string? filter)
    {
        var query = Set.Where(m => m.Entitytype == entitytype);
        var total = query.LongCount();
        var rows = query.OrderBy(m => m.Id)
            .Skip(pageable.Page * pageable.Size).Take(pageable.Size)
            .Select(m => new
            {
                m.Id,
                Level = m.PlaningLevel != null ? m.PlaningLevel.DesLevel : null,
                User = m.Tashilgar != null ? m.Tashilgar.NameF + " " + m.Tashilgar.FamilyF : null,
                m
            }).ToList();

        var content = rows.Select(r => new ControlPanelDto(
            r.Id, r.Level, r.User,
            r.m.Regdatestart?.ToString(), r.m.Regdateend?.ToString(),
            r.m.ValdateStart?.ToString(), r.m.Valdateend?.ToString(),
            r.m.Yearvalspec?.ToString(), r.m.Monthvalspec?.ToString(),
            r.m.Entitytype, r.m.Alllevelsallowedspec, r.m.PlaningLevel?.Id,
            r.m.Alltashilgarsallowedspec, r.m.Tashilgar?.UserId,
            r.m.Val_month?.ToString(), r.m.AllowedPlaningLevelIds?.ToString())).ToList();
        return ToPage(content, total, pageable.Size);
    }

    public ControlPanel? FindByEntityType(int entitytype) =>
        Set.FirstOrDefault(m => m.Entitytype == entitytype);

    public List<ControlPanel> FindByEntityTypeAndTashilgarIdAndLevelId(int entitytype, long tashilgarId, long levelId) =>
        Set.Where(m => m.Entitytype == entitytype
            && (m.Alltashilgarsallowedspec == true || (m.Tashilgar != null && m.Tashilgar.UserId == tashilgarId))
            && (m.Alllevelsallowedspec == true || (m.PlaningLevel != null && m.PlaningLevel.Id == levelId)))
            .ToList();

    public List<ControlPanel> FindByEntityType2AndTashilgarIdAndLevelId(int entitytype, long tashilgarId, long levelId) =>
        FindByEntityTypeAndTashilgarIdAndLevelId(entitytype, tashilgarId, levelId);
}
