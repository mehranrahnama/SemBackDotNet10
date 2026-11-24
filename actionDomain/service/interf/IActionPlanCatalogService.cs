using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
// Mechanical conversion from Java (service/interf/IActionPlanCatalogService.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.Base.Service.Inter;
using SemBackDotNet10.Base.model.dto;
using Microsoft.AspNetCore.Http;
using System.Collections.ObjectModel;
using Common.utils;




namespace SemBackDotNet10.actionDomain.service.interf;

/// <summary>معادل IActionPlanCatalogService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IActionPlanCatalogService : IProcessService<ActionPlanCatalog,long?,ActionPlanCatalogRepository>
{
    SearchDto loadGrid(string? body, int? page, int? size, string? orderBy, SortDirection direction);
    SearchDto loadGridByPlanId(string? body, int page, int size, string? orderBy, SortDirection direction);
    SearchDto loadGridByRole(string? body, int page, int size, string? orderBy, SortDirection direction);
    void setIsPlanned(ActionPlanCatalog actionPlanCatalog);
    ActionPlanCatalog ConfirmReplan(ActionPlanCatalogDto actionPlanCatalogDto);
    void rejectReplan(long? actionPlanCatalogId);
    ActionPlanCatalog create(ActionPlanCatalogDto actionPlanCatalogDto, HttpRequest request);
    ActionPlanCatalogDto mapToDto(ActionPlanCatalog actionPlanCatalog);
    ActionPlanCatalog mapToEntity(ActionPlanCatalogDto actionPlanCatalogDto);
    void delete(long? id);
    ActionPlanCatalog findByIdOrElseThrow(long? id);
    ActionPlanCatalog findByActionPlanIdOrElseThrow(long? ActionPlanId);
    ActionPlanCatalog edit(ActionPlanCatalogDto actionPlanCatalogDto);
    Dictionary<string?,string?> getDates(long? id);
    ActionPlanCatalog editDeleteStatus(ActionPlanCatalogDto actionPlanCatalogDto);
    List<ActionPlanCatalog> findByPlanIdAndLevelIdAndPayeshgar(long? planId, long? levelId, long? payeshgarId);
    List<ActionPlanCatalog> findAllConnectedActions();
    ActionPlanCatalogDto findActionPlanCatalogById(long? id);
    void editConfirmStatus(long? id, string? outCome);
    bool expireActionPlanCatalogTasks(long? tenantId);
    void changeModir(long? oldModirId, long? newModirId, long? planId);
    void changeTashilgar(long? oldTashilgarId, long? newTashilgarId, long? planId);
    void checkExistOrCreateActionPlan(ActionPlanCatalog actionPlanCatalog);
    SearchDto getActionPlanCatalogModir(long? levelId, int page, int size, string? orderBy, SortDirection direction);
    void assignFacilitatorAndManager(long? planningLevelId, long? facilitatorId, long? managerId);
    // TODO(signature): ActionPlanCatalog baseActionPlanCatalog );
    void updatePlanned(bool planned, long? id);
    void resetPlanningFlags(Collection<long> ids);
    void validateActionDateRange(string? startDate, string? endDate);
    bool checkIsPlanned(long? actionPlanCatalogId);
    void updateReplannedAndReplannedStatus(ActionPlanCatalog actionPlanCatalog, string? newEndDate);
    long? findCatalogPlanningLevelIdById(long? actionPlanCatalogId);
    List<ActionPlanCatalog> findPlannedCatalogsWithCorruptedPlanningState();
    List<ActionPlanCatalog> findAllWithTaskUrlIsNotNull();
}
