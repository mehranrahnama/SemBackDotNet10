using System.Collections.Generic;
using System.Linq;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.handler;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.factory;

public class ActionConfirmStatusHandlerFactory
{
    private readonly Dictionary<string, IConfirmStatusHandler> _handlerMap;

    public ActionConfirmStatusHandlerFactory(IEnumerable<IConfirmStatusHandler> handlers)
    {
        _handlerMap = handlers.ToDictionary(handler => handler.entity());
    }

    public IConfirmStatusHandler getHandler(string entityName)
    {
        if (!_handlerMap.TryGetValue(entityName, out var handler))
            throw new BusinessException("No ConfirmStatusHandler found for entity: " + entityName);
        return handler;
    }
}
