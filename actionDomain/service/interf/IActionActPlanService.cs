// Mechanical conversion from Java (service/interf/IActionActPlanService.java). Complex bodies are marked with TODO.
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

using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;

using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.actionDomain.controller.dto.actionActPlanDtos;
using SemBackDotNet10.Base.Service.Inter;


namespace SemBackDotNet10.actionDomain.service.interf;

/// <summary>معادل IActionActPlanService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IActionActPlanService : IProcessService<ActionActPlan,long?,ActionActPlanRepository>
{
    ActionActPlan findByIdOrElseThrow(long? id);
    // SearchDto getDataActionActPlan(String body, int page, int size, String orderBy, SortDirection direction);
    void createActionActPlan(ActionPlanCatalog actionPlanCatalog);
    HashSet<ActionActPlanDataDto> getActionActPlan(long? actionPlanCatalogId);
    void updatePlanValues(List<UpdateActionActPlanDto> dtos, long? actionPlanCatalogId, long? strategyObjectId);
    void updateActionActPlan(ActionPlanCatalog actionPlanCatalog);
    // TODO(signature): );
    void calculationDefaultPlanValue(string? body);
    SearchDto findAllActionActPlan(string? body, int page, int size, string? orderBy, SortDirection direction);
    SearchDto findAll(long? actionPlanCatalogId, long? levelId, string? strategyObjectTitle, int page, int size, string? orderBy, SortDirection direction);
    SearchDto findAllByActionPlanCatalogId(long? actionPlanId, int page, int size, string? orderBy, SortDirection direction);
    SearchDto findAllByActionPlanCatalogId_V2(long? actionPlanId, int page, int size, string? orderBy, SortDirection direction);
    void EditActionActPlan(List<ActionActPlanDto> actPlanDtoList);
    void calculationPlanValue(long? actionPlanCatalogId, long? levelId, long? activePlanId, bool isPlaning);
    SearchDto loadGrid(Dictionary<string?,string?> headers, string? body, int page, int size, string? orderBy, SortDirection direction);
    SearchDto relatedValueGrid(string? body, int page, int size, string? orderBy, SortDirection direction);
    ActionActPlan editDeviation(ActionActPlanDto actionActPlanDto);
    ActionActPlanDto editCorrection(ActionActPlanDto actionActPlanDto);
    ActionActPlanDto mapToDto(ActionActPlan actionActPlan);
    void editAllActualValueAndBudget(List<ActionActPlanDto> actionActPlanDtoList);
    void setTaskUrl(List<ActionActPlanDto> actionActPlanDtoList);
    void extendEndDateOfExpiredProjectByFilter(string? body);
    void extendEndDateOfExpiredProject();
    void changeActionActPlanSchedule(List<ActionActPlanDto> actionActPlanDtoList);
    SearchDto loadReplanGrid(string? body, int page, int size, string? orderBy, SortDirection direction);
    SearchDto loadReplanCartableGrid(string? body, int page, int size, string? orderBy, SortDirection direction);
    ActionPlanCatalogReplanDto loadReplanDataByActionPlanCatalog(long? actionPlanCatalogId);
    // TODO(signature): );
    void saveReplanValues(List<ActionActReplanDto> actionActReplanDtos, Dictionary<string?,string?> actionInfo);
    void editStatus(ActionActPlanDto actionActPlanDto);
    ActionProgressDto? getActionActPlan(string? body);
    List<ActionActPlanDto> getCorrectionByActPlanId(long? id);
    //    @Transactional(rollbackFor = Exception.class)
    //    SearchDto CreateOrGetDataActionActPlanV2(String body, int page, int size, String orderBy, SortDirection direction) throws Exception;
    SearchDto CreateOrGetDataActionActPlan(string? body, int page, int size, string? orderBy, SortDirection direction);
    //List<ActionActPlan> getAllByActionPlanIds(List<ActionPlan> actionPlanList);
    List<ActionActPlanDto> findAllActionActPlansOfActionPlan(long? actionActPlanId);
    void removeExpiredActionActPlan(List<ActionActPlan> actionActPlans);
    void restoreActionActPlans(ActionPlanCatalog actionPlanCatalog, long? activePlanId);
    void restoreActionActPlans(long? actionPlanCatalogId);
    void restoreDeletedActionActPlan(ActionPlanCatalog actionPlanCatalog);
    void removeActiveActionActPlans(long? actionPlanCatalogId, long? activePlanId);
    void deleteActionActPlanByActionPlanCatalog(long? actionPlanCatalogId);
    decimal? getCorrectionActualValueForHOSCO(long? actionActPlanId);
    void deleteActionPlanCatalog(long? actionPlanCatalogId);
    string? mapOrderByToQueryOrderBy(string? orderBy);
    bool? userIsAdmin(long? userId);
    void removeDefaultPlanning(long? actionPlanCatalogId);
    void copyAndCreateActionActPlan(List<ActionPlanCatalog> actionPlanCatalogs, ActionPlanCatalog baseActionPlanCatalog);
    void copyActionActPlansForUpperLevels(long? actionPlanCatalogId, List<long?> ancestorIds);
    List<ActionActPlan> findAllWithTaskUrlIsNotNull();
    List<ActionActPlan> findAllByActionPlanCatalogId(long? catalogId);
}
