using Common.utils;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Data;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.SecurityDomain.Service.Impl;

public sealed class ControlPanelPageReader(SemDbContext db)
{
    public async Task<ServicePage<ControlPanelDto>> FindAsync(int entityType, string? filter, int page, int size,
        string? orderBy, SemBackDotNet10.Common.enums.SortDirection direction, CancellationToken ct)
    {
        var request = PageUtils.GetInstance(page, size, orderBy ?? "id", direction);
        var query = db.Database.SqlQuery<ControlPanelRow>($"""
            SELECT CAST(m.id AS bigint) AS Id, p.DesLevel AS PlaningLevelDes,
                   CONCAT(t.NameF, ' ', t.FamilyF) AS UserName,
                   m.RegDate_Start AS Regdatestart, m.RegDate_End AS Regdateend,
                   m.ValDate_Start AS Valdatestart, m.ValDate_End AS Valdateend,
                   m.Year_Val_Spec AS Yearvalspec, m.Month_Val_Spec AS Monthvalspec,
                   CAST(m.EntityType AS int) AS Entitytype, m.IsAllLevelsAllowed_Spec AS Alllevelsallowedspec,
                   CAST(p.LevelId AS bigint) AS Levelidallowedspec, m.IsAllTashilgarsAllowed_Spec AS Alltashilgarsallowedspec,
                   CAST(t.UserId AS bigint) AS Tashilgaridallowedspec, m.Val_Month AS Val_month,
                   m.allowedPlaningLevelIds AS AllowedPlaningLevelIds
            FROM dbo.tblControlPanel AS m
            LEFT JOIN dbo.tblPlaningLevel AS p ON p.LevelId = m.LevelIdAllowed_Spec
            LEFT JOIN dbo.tblUser AS t ON t.UserId = m.TashilgarIdAllowed_Spec
            WHERE m.EntityType = {entityType}
            """);
        if (filter is not null) query = query.Where(row => false);
        var total = await query.LongCountAsync(ct);
        var rows = await PageUtils.Apply(query, request).ToListAsync(ct);
        var result = rows.Select(row => new ControlPanelDto(row.Id, row.PlaningLevelDes, row.UserName,
            row.Regdatestart, row.Regdateend, row.Valdatestart, row.Valdateend, row.Yearvalspec, row.Monthvalspec,
            row.Entitytype, row.Alllevelsallowedspec, row.Levelidallowedspec, row.Alltashilgarsallowedspec,
            row.Tashilgaridallowedspec, row.Val_month, row.AllowedPlaningLevelIds)).ToList();
        return new(result, total, request.Page, request.Size);
    }

    public sealed class ControlPanelRow
    {
        public long Id { get; set; }
        public string? PlaningLevelDes { get; set; }
        public string? UserName { get; set; }
        public string? Regdatestart { get; set; }
        public string? Regdateend { get; set; }
        public string? Valdatestart { get; set; }
        public string? Valdateend { get; set; }
        public string? Yearvalspec { get; set; }
        public string? Monthvalspec { get; set; }
        public int? Entitytype { get; set; }
        public bool? Alllevelsallowedspec { get; set; }
        public long? Levelidallowedspec { get; set; }
        public bool? Alltashilgarsallowedspec { get; set; }
        public long? Tashilgaridallowedspec { get; set; }
        public string? Val_month { get; set; }
        public string? AllowedPlaningLevelIds { get; set; }
    }
}
