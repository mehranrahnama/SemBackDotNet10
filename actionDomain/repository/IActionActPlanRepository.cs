using global::SemBackDotNet10.Common.Pagination;
// Mechanical conversion from Java (repository/ActionActPlanRepository.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.baseDomain.repository;
using Pageable = SemBackDotNet10.Common.Pagination.Pageable;
using System.Linq.Dynamic.Core;




namespace SemBackDotNet10.actionDomain.repository;

/// <summary>معادل ActionActPlanRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IActionActPlanRepository : ISemRepository<ActionActPlan,long?>
{
    // TODO(signature): Pageable pageable);
    global::SemBackDotNet10.Common.Pagination.PagedResult<ActionActPlanDto> findAllByActionPlanCatalogId(long? actionPlanCatalogId, Pageable pageable);
    global::SemBackDotNet10.Common.Pagination.PagedResult<ActionActPlanDto> findAllByActionPlanCatalogId_v2(long? actionPlanCatalogId, Pageable pageable);
    // TODO(signature): List<ActionActPlan> findByActionPlanCatalogIdAndLevelIdGreaterThanActivePlanId(
    // TODO(signature): List<ActionActPlan> findByActionPlanCatalogIdAndLevelIdLessThanActivePlanId(
    // TODO(signature): "act.id, act.actionPlanCatalog.id, " + "cat.description, " + "obj.title, " + "concat(ap.payeshgar.nameF ,' ', ap.payeshgar.familyF) ," + "act.planValue, " + "ac
    List<ActionProgressDto> findByActionIdAndPlanId(long? id, long? planId);
    long? getFinalPlanValue(long? actionPlanCatalogId);
    decimal? getFinalPlanValueByActionPlanCatalogAndPlanId(long? actionPlanCatalogId, long? planId);
    long? getFinalActualValue(long? actionPlanCatalogId);
    List<ActionActPlan> findFutureActs(long? actionPlanCatalogId, long? month, long? planId);
    // TODO(signature): ) Page<RelatedActsDto> relatedValueGrid(Pageable pageable, @Param("actId") Long actId);
    global::SemBackDotNet10.Common.Pagination.PagedResult<ActionActReplanDto> loadReplanGrid(Pageable pageable, long? apcId);
    global::SemBackDotNet10.Common.Pagination.PagedResult<ActionActReplanDto> getActiveActPlanByActionPlanCatalogId(Pageable pageable, long? apcId);
    // TODO(signature): "order by  act.planPeriod.planId , act.month asc") Page<ActionActReplanDto> getExpiredActPlanByActionPlanCatalogId(Pageable pageable, @Param("apcId") Long apcId
    List<ActionActPlan> findByActionPlanCatalog_Id(long? id);
    // TODO(signature): "where pln.id=:actionPlanCatalogId " + "and aap.month > :month and aap.deleted = false and aap.planPeriod.planId = :planId") "where aap.actionPlanCatalog.id=:ac
    // TODO(signature): "act.id , " + "act.planPeriod.planId ," + "act.month," + "act.planValue , " + "act.id , " + "act.actualValue ," + "act.actionPlanCatalog.planned ," + " apc.id )
    // TODO(signature): " order by act.planPeriod.planId asc , act.month asc ") List<ActionActPlanDto> findAllByActionPlanCatalog_Id(@Param("actionPlanCatalogId") Long actionPlanCatalo
    // TODO(signature): "and aap.deleted = false") List<ActionActPlan> findAllByActionPlanCatalogAndDeletedIsFalse(@Param("actionPlanCatalogId") Long actionPlanCatalogId ,@Param("activ
    List<ActionActPlan> findAllByActionPlanCatalogAndDeletedIsTrue(ActionPlanCatalog actionPlanCatalog);
    List<ActionActPlan> findByActionPlanCatalogAndPlanPeriodAndMonthAndDeletedIsFalse(ActionPlanCatalog actionPlanCatalog, PlanPeriod planPeriod, long? Month);
    // @Query(value = "select aap from ActionActPlan aap where aap.actionPlanCatalog.id= :actionPlanCatalogId and aap.planPeriod.planId >= :activePlanId")
    List<ActionActPlan> findByActionPlanCatalogIdAndGreaterThanPlanId(long? actionPlanCatalogId, long? activePlanId);
    List<ActionActPlanDto> findAllByStrategyObjId(long? strategyObjId);
    // @Query("select act from ActionActPlan act where act.id=:actionActPlanId and act.deleted is false" )
    ActionActPlan findByIdAndDeletedIsFalse(long? actionActPlanId);
    List<ActionActPlan> findByActionPlanCatalogIdAndDeletedIsFalse(long? actionPlanCatalogId);
    List<ActionActPlan> findByActionPlanCatalogId(long? actionPlanCatalogId);
    // TODO(signature): );
    // TODO(signature): );
    List<ActionActPlan> findByActionPlanCatalog_IdAndMonthGreaterThanAndDeletedFalse(long? actionPlanCatalogId, long? month);
    // TODO(signature): "where a.planPeriod.planId = :planId " + "and (:isAdmin is true or a.actionPlanCatalog.planingLevel.id in :userLevelIds) " + "and a.month BETWEEN :fromMonth and
    // TODO(signature): "act.confirmstatus = :confirmStatus " + "and act.deleted = false " + "and act.actualValue IS Not NULL " + "and act.planPeriod.planId =:planId " + "and act.taskU
    // TODO(signature): "act.actionPlanCatalog.id = :catalogId and act.month = :month and act.planPeriod.planId = :planId") "act.actionPlanCatalog.id = :catalogId and act.month = :mont
    // TODO(signature): );
    // TODO(signature): """) boolean existsByActionPlanCatalogIdAndDeletedIsFalse(Long actionPlanCatalogId);
    void softDeleteAllByActionPlanCatalogId(long? actionPlanCatalogId);
    HashSet<ActionActPlanDataDto> findAllByActionPlanCatalogId(long? actionPlanCatalogId);
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
    void deleteAllExpiredRecordsByActionPlanCatalogId(long? actionPlanCatalogId);
    List<long?> findExpiredIdsByActionPlanCatalogId(long? actionPlanCatalogId);
    void removeActiveActionActPlansByActionPlanCatalog(long? actionPlanCatalogId);
    void restoreRemovedActionActPlanByActionPlanCatalog(long? actionPlanCatalogId);
    void updatePlanValuesToNullByActionPlanCatalogId(long? actionPlanCatalogId);
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
    List<ActionActPlan> findAllWithTaskUrlIsNotNull();
    // TODO(signature): );
    // TODO(signature): """) int softDeleteByActionPlanCatalogId(@Param("actionPlanCatalogId") Long actionPlanCatalogId);
    // TODO(signature): """) List<ActionActPlan> findActionPlansByCatalogOrderByPlanIdAscMonthAsc(@Param("catalogId") Long catalogId);
}
