// Mechanical conversion from Java (service/interf/IActionPlanService.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.controller.dto;
using SemBackDotNet10.actionDomain.mapper;
using SemBackDotNet10.actionDomain.repository;
using SemBackDotNet10.actionDomain.service.interf;
using SemBackDotNet10.actionDomain.domain;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.context;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.factory;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.handler;
using global::Common.model.entity;
using global::Common.model.dto;
using global::Common.controller.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;
using global::Common.messages;
using global::Common.repository;
using global::Common.service.interf;
using global::Common.service.impl;
using SemBackDotNet10.Base.Service.Inter;
using SemBackDotNet10.webserviceDomain.domain.dto;


namespace SemBackDotNet10.actionDomain.service.interf;

/// <summary>معادل IActionPlanService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IActionPlanService : IProcessService<ActionPlan,long?,ActionPlanRepository>
{
    ActionPlan findByIdOrElseThrow(long? id);

    // TODO(signature): );
    // TODO(signature): );
    ActionPlanDto mapToDto(ActionPlan actionPlan);
    ActionPlan edit(RequestActionPlanDto actionPlan);
    void delete(long? id);
    void deleteActionPlanByActionPlanCatalogId(long? actionPlanCatalogId);
    List<long?> getUserLevelIds(long? userId);
    // TODO(signature): );
    // TODO(signature): );
    void editAllWeights(List<ActionPlanDto> actionPlanDtos);
    void editDeleteStatus(string? body);
    Dictionary<string?,object?> getCounts(string? body);
    List<ActionPlan> findByCatalogId(long? catalogId);
    ActionPlanDto findActionPlanById(long? id);
    void editConfirmStatus(long? id, string? outCome);
    void editConfirmStatusForDelete(ActionPlan actionPlan);
    // TODO(signature): ) throws Exception;
    long? CountActiveActions(long? planId);
    long? CountActiveActionsByPlanIdLevelId(long? planId, long? levelId);
    // @Query("select ap from ActionPlan ap where ap.actionPlanCatalog.id = :actionPlanCatalogId and ap.planPeriod.id = :planId")
    List<ActionPlan> findByActionPlanCatalogIdAndPlanId(long? actionPlanCatalogId, long? planId);
    List<ActionPlan> findByActionPlanCatalogIdAndPlanIAndDeletedIsFalse(long? actionPlanCatalogId, long? planId);
    ActionPlan insertNewActionPlanFromPreviewsActionPlan(StrategyObject strategyObject, long? planId, ActionPlan actionPlan);
    bool expireActionPlanTasks(long? tenantId);
    void createNewActionPlanFromOldActionPlan(StrategyObject strategyObject, ActionPlanCatalog actionPlanCatalog, long? year);
    void changeTashilgar(long? oldTashilgarId, long? newTashilgarId, long? planId);
    void changeModir(long? oldModirId, long? newModirId, long? planId);
    // TODO(signature): );
    void hardDeleteActionPlan(List<StrategyObject> deletedStgList);
    void assignFacilitatorAndManager(long? planningLevelId, long? facilitatorId, long? managerId);
    void updateConfirmStatusAndTaskUrlAfterPlanning(long? actionPlanCatalogId, long? strategyObjectId);
    bool isAddToSupremeLevel(long? actionPlanCatalogId);
    List<ActionPlan> findAllWithTaskUrlIsNotNull();
    // TODO(signature): String newEndDate);
    List<ActionPlanWebServiceDto> findActionPlanForWebService(long? planId);
}
