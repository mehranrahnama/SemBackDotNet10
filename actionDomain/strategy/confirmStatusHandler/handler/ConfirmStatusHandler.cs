using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.context;

namespace SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.handler;

public interface IConfirmStatusHandler
{
    string entity();
    void updateCartableStatus(CartableStatusContext context);
}
