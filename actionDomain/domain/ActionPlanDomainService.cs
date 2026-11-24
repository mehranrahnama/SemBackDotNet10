// Mechanical conversion from Java (domain/ActionPlanDomainService.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.actionDomain.model.dto.actionPlanDtos;


namespace SemBackDotNet10.actionDomain.domain;

/// <summary>معادل ActionPlanDomainService.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class ActionPlanDomainService
{
    private readonly ActionPlanRepository _repository;
    private readonly ActionPlanRepository _actionPlanRepository;
    private readonly ActionPlanYearPcnWriter _yearPcnWriter;

    public ActionPlanDomainService(ActionPlanRepository repository, ActionPlanRepository actionPlanRepository,
        ActionPlanYearPcnWriter yearPcnWriter)
    {
        _repository = repository;
        _actionPlanRepository = actionPlanRepository;
        _yearPcnWriter = yearPcnWriter;
    }

    ActionPlanYearConfigProjection validateAndGetYearPlanInfo(long? catalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogId)
        throw new NotImplementedException();
    }
    List<ActionPlanYearConfigProjection> getYearPlanInfos(long? catalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogId)
        throw new NotImplementedException();
    }
    // TODO(signature): Long catalogId ) { Map<Long, List<ActionPlanYearConfigProjection>> groupedByPlanId = infos.stream() .collect(Collectors.groupingBy(ActionPlanYearConfigProjectio
    // TODO(signature): ) { List<ActionPlanYearConfigProjection> plansInfo = getYearPlanInfos(catalogId); validateYearPlanConsistency(plansInfo, catalogId); }
    List<ActionPlan> findRelatedActionPlans(long? actionPlanId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanId)
        throw new NotImplementedException();
    }
    void saveAll(List<ActionPlan> entities)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — entities)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { return UriComponentsBuilder.fromHttpUrl(SettingTaskURL) .path("Action/ActionManagement/ActionManagementCartable") .queryParam("entityId", id) .queryParam("o
    List<ActionPlan> findByActionPlanCatalogIds(HashSet<long?> catalogIds)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogIds)
        throw new NotImplementedException();
    }
    void updateYearCompletionValuesForActionPlans(Dictionary<long?,decimal?> yearPlanValues, long? catalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogId)
        throw new NotImplementedException();
    }
    public void syncYearPcnByPlanId(long? catalogId, long? planId,
        decimal? yearStartPcn, decimal? yearCompletePcn)
    {
        try
        {
            _yearPcnWriter.UpdateYearPcnByCatalogIdAndPlanId(
                catalogId, planId, yearStartPcn, yearCompletePcn);
        }
        catch (Exception)
        {
            // Java masks all repository exceptions with this business message.
            throw new SemBackDotNet10.Common.exception.BusinessException(
                "خطایی در به‌روزرسانی مقادیر PCN سال جاری رخ داد!");
        }
    }
    // TODO(signature): ) { Map<Long, YearPcn> yearPcnMap = new LinkedHashMap<>(); if (endYear < startYear) { return yearPcnMap; } if (currentYear < startYear || currentYear > endYear 
    long? yearToPlanId(long? year)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — year)
        throw new NotImplementedException();
    }
    void updateAddToSupremeLevelByCatalog(long? actionPlanCatalogId, bool addToSupremeLevel)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — addToSupremeLevel)
        throw new NotImplementedException();
    }
    Dictionary<long?,ActionPlanYearConfigProjection> validateAndGetYearPlanInfoByPlanId(long? catalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogId)
        throw new NotImplementedException();
    }
}
