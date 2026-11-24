// Mechanical conversion from Java (repository/ActionPlanRepository.java). Complex bodies are marked with TODO.
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

using SemBackDotNet10.Base.repository;


namespace SemBackDotNet10.actionDomain.repository;

/// <summary>معادل ActionPlanRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface ActionPlanRepository : ISemRepository<ActionPlan, long?>
{
    void updateFacilitatorAndManagerByPlanningLevelId(long? levelId, long? facilitatorId, long? managerId);

    List<ActionPlan> findByStrategyObject_TitleAndAddToSupremeLevelTrue(string? title);
    // TODO(signature): value = """ select new com.irisaco.mss.sem.actionDomain.model.dto.ActionPlanDto( ap.id, apc.description, pl.desThreeChar, apc.code, concat(m.nameF, ' ', m.famil
    // TODO(signature): );
    List<long?> getCurrentUserLevelIds(long? userId);
    // TODO(signature): );
    // TODO(signature): "from ActionPlan ap " + "left join ap.strategyObject so " + "left join ap.actionPlanCatalog apc " + "left join ap.modir m " + "left join ap.payeshgar p " + "lef
    // TODO(signature): "from ActionPlan ap " + "left join ap.strategyObject so " + "left join ap.actionPlanCatalog apc " + "left join ap.modir m " + "left join ap.payeshgar p " + "lef
    // @Query(value = "select a from ActionPlan a where a.strategyObject.id=:objectId")
    List<ActionPlan> findAllByObjectId(long? objectId);
    ActionPlan findByActionPlanCatalog_IdAndStrategyObject_IdAndStrategyObject_PlaningLevel_IdAndPlanPeriod_PlanId(long? catalogId, long? objectId, long? levelId, long? planId);
    //    @Query(value = "select ap " +
    //            "from ActionPlan ap    " +
    //            "left join ap.actionPlanCatalog cat   " +
    //            "left join ap.actionActPlans act   " +
    //            "left join ap.strategyObject obj   " +
    //            "left join ap.planPeriod pp   " +
    //            "left join ap.planingLevel pl   " +
    //            "where  " +
    //            "ap.actionPlanCatalog.id=:actionPlanCatalogId " +
    //            "and ap.strategyObject.id=:strategyObjectId " +
    //            "and pl.id=:levelId  " +
    //            "and ap.planPeriod.planId=:planId ")
    //    ActionPlan findByStrategyObjectCatalogIdAndLevelIdAndPlanId (@Param("strategyObjectId") Long strategyObjectId,
    //                                                                 @Param("planId") Long planId, @Param("actionPlanCatalogId") Long actionPlanCatalogId,
    //                                                               @Param("levelId") Long levelId);
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id=:catalogId")
    List<ActionPlan> findByCatalogId(long? catalogId);
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id=:catalogId and ap.deleteStatus = false")
    List<ActionPlan> findByCatalogIdAndDeleteStatusIsFalse(long? catalogId);
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id=:catalogId and (ap.deleteStatus = false or ap.confirmstatus != 50)")
    List<ActionPlan> findByCatalogIdAndDeletedIsFalseOrNotFinalState(long? catalogId);
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id=:catalogId and ap.confirmstatus != 50")
    List<ActionPlan> findByCatalogIdAndConfirmstatus(long? catalogId);
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id=:actionPlanCatalogId and ap.deleteStatus = true and ap.confirmstatus!= 50L")
    List<ActionPlan> findByActionPlanCatalogIdAndDeleteStatusIsTrue(long? actionPlanCatalogId);
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id=:actionPlanCatalogId and ap.deleteStatus = true")
    List<ActionPlan> findByActionPlanCatalogIdAndDeleted(long? actionPlanCatalogId);
    List<ActionPlan> findByActionPlanCatalogIdAndDeleteStatusIsFalse(long? actionPlanCatalogId, long? planId);
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id=:actionPlanCatalogId and ap.strategyObject.id = :strategyObjId and ap.deleteStatus = false")
    List<ActionPlan> findByActionPlanCatalogIdAndStrategyObjIdAndDeleteStatusIsFalse(long? actionPlanCatalogId, long? strategyObjId);
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id=:actionPlanCatalogId and ap.strategyObject.id = :strategyObjId and ap.deleteStatus = true")
    ActionPlan findByActionPlanCatalogIdAndStrategyObjectIdAndDeleteStatusIsTrue(long? actionPlanCatalogId, long? strategyObjId);
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id=:actionPlanCatalogId and ap.strategyObject.id = :strategyObjId and ap.deleteStatus = false")
    ActionPlan findFirstByActionPlanCatalogIdAndStrategyObjectIdAndDeleteStatusIsFalse(long? actionPlanCatalogId, long? strategyObjId);
    ActionPlanDto findActionPlanById(long? id);
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id=:actionPlanCatalogId and ap.planPeriod.planId = :planId")
    List<ActionPlan> findByActionPlanCatalogIdAndPlanId(long? actionPlanCatalogId, long? planId);
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id=:actionPlanCatalogId and ap.planPeriod.planId = :planId and ap.deleteStatus = false")
    List<ActionPlan> findByActionPlanCatalogIdAndPlanIAndDeletedIsFalse(long? actionPlanCatalogId, long? planId);
    // @Query(value = "exec  [dbo].[CountActiveActions] :planId"   , nativeQuery = true)
    long? CountActiveActions(long? planId);
    // @Query(value = "exec  [dbo].[CountActiveActionsByPlanIdLevelId] :planId , :levelId"   , nativeQuery = true)
    long? CountActiveActionsByPlanIdLevelId(long? planId, long? levelId);
    // TODO(signature): "where ap.actionPlanCatalog.id=:catalogId " + "and ap.strategyObject.id = :strategyObjId " + "and ap.planingLevel.id = :levelId " + "and ap.deleteStatus = true"
    // @Query(value = "select ap from ActionPlan ap where ap.strategyObject.id = :strategyObjectId and ap.deleteStatus = false")
    List<ActionPlan> findByStrategyObjectIdAndDeleteStatusIsFalse(long? strategyObjectId);
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id = :actionPlanCatalogId and ap.deleteStatus = true and ap.confirmstatus != 50L")
    List<ActionPlan> findByActionPlanCatalogIdAndDeletedIsTrue(long? actionPlanCatalogId);
    // TODO(signature): "ap.confirmstatus = :confirmStatus " + "and ap.deleteStatus = false " + "and ap.actionPlanCatalog.planned is true " + "and ap.confirmstatus not in :ignoredConfi
    // TODO(signature): "ap.confirmstatus = :confirmStatus " + "and ap.deleteStatus = false " + "and ap.actionPlanCatalog.planned is true " + "and ap.confirmstatus not in :ignoredConfi
    // TODO(signature): "ap.confirmstatus = :confirmStatus " + "and ap.deleteStatus = false " + "and ap.actionPlanCatalog.planned is true " + "and ap.confirmstatus not in :ignoredConfi
    // @Query(value = "select ap from ActionPlan ap where ap.actionPlanCatalog.id = :actionPlanCatalogId and ap.deleteStatus is false")
    List<ActionPlan> getByActionPlanCatalogIdAndDeletedIsFalse(long? actionPlanCatalogId);
    ActionPlan findByActionPlanCatalogIdAndPlanIdAndStrategyObjectTitleAndDeletedIsFalse(long? actionPlanCatalogId, long? planId, string? StrategyObjectTitle);
    // @Query(value = "select ap from ActionPlan ap where ap.payeshgar.id = :payeshgarId and ap.planPeriod.planId = :planId and ap.deleteStatus is false")
    List<ActionPlan> findByPayeshgarIdAndPlanIdAndDeletedIsFalse(long? payeshgarId, long? planId);
    // @Query(value = "select ap from ActionPlan ap where ap.modir.id = :ModirId and ap.planPeriod.planId = :planId and ap.deleteStatus is false")
    List<ActionPlan> findByModirIdAndPlanIdAndDeletedIsFalse(long? ModirId, long? planId);
    // @Query(value = "select ap from ActionPlan ap where ap.strategyObject.id in :strategyObjectIds and ap.deleteStatus = true")
    List<ActionPlan> findAllByStrategyObjectIdAndDeletedIsTrue(List<long?> strategyObjectIds);
    List<ActionPlan> findActionPlanByIdAndStrategyObjectAndActionPlanCatalog(long? id);
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
    List<ActionPlan> findAllWithTaskUrlIsNotNull();
    // TODO(signature): );
    // TODO(signature): SELECT ap FROM ActionPlan ap WHERE ap.strategyObject.id = ( SELECT a.strategyObject.id FROM ActionPlan a WHERE a.id = :id ) AND ap.actionPlanCatalog.id = ( SELE
    // TODO(signature): );
    bool existsByActionPlanCatalogIdAndLevelId(long? catalogId, long? levelId);
    // TODO(signature): """) List<ActionPlanYearConfigProjection> findYearPlanInfoByCatalogId(@Param("catalogId") Long catalogId);
    List<ActionPlan> findAllByActionPlanCatalogIds(HashSet<long?> catalogIds);
    List<ActionPlan> findAllByCatalogIdAndDeletedFalse(long? catalogId);
    // TODO(signature): );
    // TODO(signature): """) List<String> findAllDesLevelsByIds(Set<Long> catalogIds);
    // TODO(signature): );
    // TODO(signature): """) List<ActionPlan> findBrokenActionPlans();
    // TODO(signature): Long strategyObjectId );
    List<ActionPlan> findByActionPlanCatalog_IdOrderByPlanPeriod_PlanIdAsc(long? actionPlanCatalogI);
    ActionPlan findByActionPlanCatalog_IdAndPlanPeriod_PlanId(long? actionPlanCatalogId, long? planI);
    // TODO(signature): );
    // TODO(signature): );
}
