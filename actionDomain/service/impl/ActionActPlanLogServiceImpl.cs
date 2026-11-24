#nullable enable
using Microsoft.Extensions.Logging;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.actionDomain.repository;
using SemBackDotNet10.actionDomain.service.interf;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public sealed class ActionActPlanLogServiceImpl(
    ActionActPlanLogRepository repository,
    ILogger<ActionActPlanLogServiceImpl> logger) : IActionActPlanLogService
{
    public ActionActPlanLogRepository GetRepository() => repository;
    public ActionActPlanLogRepository getRepository() => GetRepository();

    public void saveAllActionActPlanLogs(List<ActionActPlanLog> actionActPlanLogs)
    {
        ArgumentNullException.ThrowIfNull(actionActPlanLogs);
        logger.LogDebug("Saving action act plan logs");
        try
        {
            repository.SaveAllAndCommit(actionActPlanLogs);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Saving action act plan logs failed");
            // Do not disclose SQL/provider messages to the client.
            throw new BusinessException("خطا در ذخیره تاریخچه برنامه اقدام");
        }
    }

    public void findAndDeleteActionActPlanLog(long? actionPlanCatalogId)
        => repository.DeleteByCatalogAndCommit(actionPlanCatalogId);
}
