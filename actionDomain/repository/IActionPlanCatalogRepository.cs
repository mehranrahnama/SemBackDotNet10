// Mechanical conversion from Java (repository/ActionPlanCatalogRepository.java). Complex bodies are marked with TODO.
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
using System.Collections.ObjectModel;



namespace SemBackDotNet10.actionDomain.repository;

/// <summary>معادل ActionPlanCatalogRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface ActionPlanCatalogRepository : global::SemBackDotNet10.Base.repository.ISemRepository<ActionPlanCatalog,long?>
{
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): "ac.id," + "ac.code, " + "ac.description," + "ac.startDate," + "ac.endDate," + "ac.totalBudget," + "ac.confirmstatus," + "pl.id," + "pl.desLevel, " + "ac.delete
    // TODO(signature): "where ap.payeshgar.id=:payeshgarId and ap.planPeriod.planId=:planId and apc.planingLevel.id=:levelId ") "where ap.payeshgar.id=:payeshgarId and ap.planPeriod.p
    // TODO(signature): "(select ap.actionPlanCatalog.id from ActionPlan ap)") "(select ap.actionPlanCatalog.id from ActionPlan ap)") List<ActionPlanCatalog> findAllConnectedActions();
    List<ActionPlanCatalog> findAllConnectedActions();
    ActionPlanCatalog findActionPlanCatalogByActionPlanId(long? actionPlanId);
    ActionPlanCatalogDto findActionPlanCatalogById(long? id);
    // TODO(signature): "(select ap.actionPlanCatalog.id from ActionPlan ap)") "(select ap.actionPlanCatalog.id from ActionPlan ap where ap.id=:ActionPlanId)") ActionPlanCatalog findAc
    // @Query(value = "select ap from ActionPlanCatalog ap where ap.modir.id = :oldModirId and ap.deleteStatus = false")
    List<ActionPlanCatalog> findByModirIdAndDeletedIsFalse(long? oldModirId);
    // TODO(signature): "where apc.confirmstatus = :confirmStatus " + "and apc.deleteStatus = false " + "and apc.confirmstatus not in :ignoredConfirmStatus " + "and apc.taskUrl is not 
    // TODO(signature): List<ActionPlanCatalog> findByConfirmStatusAndDeletedIsFalse(
    // TODO(signature): "apc.replannedStatus = :confirmStatus " + "and apc.deleteStatus = false " + "and apc.replannedStatus not in :ignoredConfirmStatus " + "and ap.planPeriod.planId 
    // @Query(value = "select ap from ActionPlanCatalog ap where ap.payeshgar.id = :oldPayeshgarId and ap.deleteStatus = false")
    List<ActionPlanCatalog> findByPayeshgarIdAndDeletedIsFalse(long? oldPayeshgarId);
    // TODO(signature): "where apc.confirmstatus = :confirmStatus " + "and apc.deleteStatus = false " + "and apc.confirmstatus not in :ignoredConfirmStatus " + "and apc.taskUrl is not 
    // TODO(signature): List<ActionPlanCatalog> findByConfirmStatusAndDeletedIsTrue(
    // TODO(signature): "from ActionPlanCatalog ap left join ap.modir m where ap.planingLevel.id = :levelId and ap.modir is not null") "from ActionPlanCatalog ap left join ap.modir m w
    // TODO(signature): "from ActionPlanCatalog ap where ap.planingLevel.id = :levelId and ap.description = :description and ap.deleteStatus = true") "from ActionPlanCatalog ap where a
    // @Query(value = "SELECT apc.confirmstatus FROM ActionPlanCatalog apc WHERE apc.id = :id ")
    long? findConfirmStatusById(long? id);
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): UPDATE ActionPlanCatalog apc SET apc.payeshgar.id = COALESCE(:facilitatorId, apc.payeshgar.id), apc.modir.id = COALESCE(:managerId, apc.modir.id) WHERE apc.plan
    void resetPlanningFlagsByIds(Collection<long> ids);
    bool findIsPlannedByActionPlanCatalogId(long? actionPlanCatalogId);
    // TODO(signature): """) Optional<Long> findCatalogLevelIdById(Long actionPlanCatalogId);
    List<ActionPlanCatalog> findAllWithTaskUrlIsNotNull();
    // TODO(signature): );
    List<ActionPlanCatalog> findPlannedCatalogsWithNoPlanningRecordsOrOnlyNullPlanningValues();
}
