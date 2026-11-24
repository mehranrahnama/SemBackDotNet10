using Common.controller;
using Common.model.entity;
using Common.service.interf;
using Common.utils;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;
using SemBackDotNet10.SecurityDomain.Infrastructure.Security;
using SemBackDotNet10.service.impl;

namespace Common.service.impl;

public sealed class CartableQueriesUnavailableException : Exception;

public sealed class CartableControllerService(
    IServiceProvider services,
    SemDbContext db,
    UserContext current,
    TimeProvider clock,
    IConfiguration configuration,
    IHostEnvironment environment) : ICartableControllerService
{
    public async Task<ServicePage<Cartable>> GetTaskListForSanganAsync(long? planId, bool? completed, string? filter,
        int page, int size, string? orderBy, string? direction, CancellationToken cancellationToken)
    {
        var pageRequest = PageUtils.GetInstance(page, size);
        var queries = services.GetService<ICartableTaskQueries>() ?? throw new CartableQueriesUnavailableException();
        var collector = new CartableTaskCollector(queries);
        var currentUserId = current.UserId;
        var now = clock.GetLocalNow().DateTime;
        var tenantId = current.TenantId;
        var delegates = await db.Delegations.AsNoTracking()
            .Where(item => item.ToUserId == currentUserId && item.DelegationStatus &&
                item.DelegationStartDate <= now && item.DelegationEndDate >= now &&
                item.FromUserId.HasValue && item.FromUser != null && item.FromUser.TenantId == tenantId)
            .Select(item => item.FromUserId!.Value).Distinct().ToListAsync(cancellationToken);

        var procedureDirection = string.Equals(direction, "ASC", StringComparison.OrdinalIgnoreCase) ? "1" : "2";
        var tasks = collector.GetTaskList(planId, currentUserId, filter, completed, orderBy, procedureDirection);
        foreach (var userId in delegates)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var task in collector.GetTaskList(planId, userId, filter, completed, orderBy, procedureDirection))
                if (!tasks.Contains(task)) tasks.Add(task);
        }

        if (environment.IsDevelopment() || environment.IsProduction())
        {
            var source = configuration["ExternalServices:TaskRouter:Url"];
            var target = configuration["ExternalServices:TaskRouter:UrlPrefix"];
            if (!string.IsNullOrEmpty(source) && target is not null)
                foreach (var task in tasks)
                    if (task.TaskUrl is not null) task.TaskUrl = task.TaskUrl.Replace(source, target, StringComparison.Ordinal);
        }

        var ordered = tasks.OrderByDescending(task => task.SentDate).ToList();
        return new ServicePage<Cartable>(ordered.Skip(pageRequest.Offset).Take(pageRequest.Size).ToList(),
            ordered.Count, pageRequest.Page, pageRequest.Size);
    }
}
