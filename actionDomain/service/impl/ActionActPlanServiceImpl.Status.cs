using System;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.context;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public void editStatus(ActionActPlanDto dto)
    {
        try
        {
            _planningUnitOfWork.Execute(() => _actionConfirmStatusHandlerFactory
                .getHandler(nameof(ActionActPlan))
                .updateCartableStatus(new CartableStatusContext(dto.Id, dto.OutCome)));
        }
        catch (BusinessException) { throw; }
        catch (Exception)
        {
            throw new BusinessException("خطایی در فرآیند بروز رسانی وضعیت پیشرفت اقدام رخ داد!");
        }
    }
}
