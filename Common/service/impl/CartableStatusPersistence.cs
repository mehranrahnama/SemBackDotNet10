using Common.model.dto;
using Common.model.entity;
using Common.repository;
using Common.service.interf;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;
using Role = SemBackDotNet10.SecurityDomain.Model.Entity.Role;

namespace Common.service.impl;

public sealed class CartableStatusPersistence(SemDbContext db) : IPersistenceSession<CartableStatus>
{
    public IQueryable<CartableStatus> Query => db.Set<CartableStatus>();
    public CartableStatus Save(CartableStatus entity)
    {
        if (entity.Id is null) db.Add(entity); else db.Update(entity);
        db.SaveChanges();
        return entity;
    }
    public void Delete(CartableStatus entity) { db.Remove(entity); db.SaveChanges(); }
}

public sealed class CartableStatusRoleHost(SemDbContext db) : ICartableStatusRoleHost
{
    public Role FindByIdOrElseThrow(long id) => db.Set<Role>().SingleOrDefault(role => role.Id == id)
        ?? throw new KeyNotFoundException($"Role {id} was not found.");
}

public sealed class CartableStatusWriteScope(SemDbContext db) : ICartableStatusWriteScope
{
    public void Execute(Action action)
    {
        if (db.Database.CurrentTransaction is not null) { action(); return; }
        var strategy = db.Database.CreateExecutionStrategy();
        strategy.Execute(() =>
        {
            using var transaction = db.Database.BeginTransaction();
            action();
            db.SaveChanges();
            transaction.Commit();
        });
    }
}

public sealed class CartableStatusQueries(SemDbContext db) : ICartableStatusQueries
{
    private IQueryable<CartableStatus> Statuses => db.Set<CartableStatus>();
    private static IQueryable<CartableStatusDto> Dtos(IQueryable<CartableStatus> query) => query.Select(status => new CartableStatusDto
    {
        Id = status.Id, EntityName = status.EntityName, OutCome = status.OutCome, OldStatus = status.OldStatus,
        NewStatus = status.NewStatus, OperationType = status.OperationType, FromRoleId = status.FromRoleId,
        FromRoleName = status.FromRole == null ? null : status.FromRole.RoleDes, ToRoleId = status.ToRoleId,
        ToRoleName = status.ToRole == null ? null : status.ToRole.RoleDes,
        OldStatusMeaning = status.OldStatusMeaning, NewStatusMeaning = status.NewStatusMeaning
    });

    public CartableStatus? FindByEntityNameIgnoreCaseAndOutComeIgnoreCaseAndOldStatus(string entityName, string outCome, long? oldStatus)
        => Statuses.SingleOrDefault(status => status.EntityName!.ToLower() == entityName.ToLower()
            && status.OutCome!.ToLower() == outCome.ToLower() && status.OldStatus == oldStatus);
    public List<CartableStatusDto> FindListByEntityNameAndOperationTypeAndOutCome(string entityName, string operationType, string outCome)
        => Dtos(Statuses.Where(status => status.EntityName == entityName && status.OperationType == operationType && status.OutCome == outCome)
            .OrderBy(status => status.OldStatus)).ToList();
    public List<CartableStatusDto> FindListByEntityName(string entityName)
        => Dtos(Statuses.Where(status => status.EntityName == entityName).OrderBy(status => status.OutCome).ThenBy(status => status.OldStatus)).ToList();
    public List<CartableStatusDto> FindAllByEntityNameAndOperationTypeAndOutCome(string entityName, string? operationType, string outCome)
        => Dtos(Statuses.Where(status => status.EntityName == entityName && (operationType == null || status.OperationType == operationType)
            && status.OutCome == outCome).OrderBy(status => status.OldStatus)).ToList();
    public CartableStatusPair? FindMaxNewStatusByEntityAndOperationAndOutcomeAndRoles(string entityName, string? operationType,
        string outCome, ISet<string> userRoles, long? confirmStatus)
    {
        var names = userRoles.ToArray();
        var matches = Statuses.Where(status => status.EntityName == entityName && (status.OperationType == operationType || status.OperationType == null)
            && status.OutCome == outCome && ((status.FromRole == null || status.FromRole.RoleName == null)
                ? status.OldStatus == confirmStatus : names.Contains(status.FromRole.RoleName)));
        var max = matches.Max(status => status.NewStatus);
        if (max is null) return null;
        var match = matches.Where(status => status.NewStatus == max).SingleOrDefault();
        return match is null ? null : new CartableStatusPair { OldStatus = confirmStatus, NewStatus = match.NewStatus };
    }
}
