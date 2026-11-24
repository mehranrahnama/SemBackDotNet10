#nullable enable

using System;
using Common.exception;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SemBackDotNet10.baseDomain.context;
using SemBackDotNet10.basicDomain.service;



namespace SemBackDotNet10.baseDomain.resolver;

public sealed class DefaultStrategyObjectResolver(
    IStrategyObjectService strategyObjectService,
    DbContext dbContext,
    ILogger<DefaultStrategyObjectResolver> logger) : IStrategyObjectResolver
{
    public StrategyObject Resolve(FindOrCreateStrategyResolutionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Equivalent to Spring Propagation.MANDATORY: do not start or commit here.
        // The service must use this same scoped DbContext/transaction.
        if (dbContext.Database.CurrentTransaction is null &&
            System.Transactions.Transaction.Current is null)
        {
            throw new InvalidOperationException(
                "An existing transaction is required to resolve a strategy object.");
        }

        try
        {
            var source = context.Source;
            if (source is null)
                throw new BusinessException("هدف استراتژیک مبدأ مشخص نشده است.");

            var planPeriod = source.PlanPeriod;
            if (planPeriod is null)
                throw new BusinessException("دوره برنامه هدف مبدأ مشخص یا بارگذاری نشده است.");

            var title = source.Title;
            if (title is null)
                throw new BusinessException("عنوان هدف استراتژیک مبدأ مشخص نشده است.");

            var viaDetails = strategyObjectService.GetStrategyObjectViaDetailsForPlanningLevels(
                planPeriod.PlanId,
                title,
                context.TargetLevel);

            return viaDetails ?? strategyObjectService.CreateStrategyObjectForLevelId(
                context.Source,
                context.TargetLevel);
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception,
                "Exception occurred when trying to resolve strategy object: {Message}",
                exception.Message);

            throw new BusinessException("خطایی در فرآیند تعیین هدف رخ داد!");
        }
    }
}
