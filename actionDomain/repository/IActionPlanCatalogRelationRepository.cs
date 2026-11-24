// Mechanical conversion from Java (repository/ActionPlanCatalogRelationRepository.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.actionDomain.model.dto.actionPlanCatalogRelationDtos;


namespace SemBackDotNet10.actionDomain.repository;

/// <summary>معادل ActionPlanCatalogRelationRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface ActionPlanCatalogRelationRepository : global::SemBackDotNet10.Base.repository.ISemRepository<ActionPlanCatalogRelation,long?>
{
    bool notExistByMainCatalogAndRelatedCatalog(long? mainCatalogId, long? relatedCatalogId);
    void deleteByRelatedActionPlanCatalogId(long? relatedCatalogId);
    long? findParentId(long? childId);
    ActionPlanCatalog findParentByChildId(long? childId);
    // TODO(signature): );
    // TODO(signature): SELECT COUNT(r.id) < 1 FROM ActionPlanCatalogRelation r WHERE r.mainActionPlanCatalog.id = :mainCatalogId AND r.relatedActionPlanCatalog.id = :relatedCatalogId 
    List<RelatedDto> getAllRelatedActionPlanCatalog(List<long?> mainIds);
    // TODO(signature): value = """ select distinct  new com.irisaco.mss.sem.actionDomain.model.dto.actionPlanCatalogRelationDtos.MainDto( map.id, map.description ) from ActionPlanCata
    // TODO(signature): );
    List<ActionPlanCatalogRelation> findByMainActionPlanCatalogId(long? mainActionPlanCatalogId);
    // TODO(signature): """) List<RelatedDto> getAllRelatedActionPlanCatalogByMainId(@Param("mainId") Long mainId);
    bool existsByMainActionPlanCatalogIdOrRelatedActionPlanCatalogId(long? mainActionPlanCatalogId, long? relatedActionPlanCatalogId);
    List<ActionPlanCatalog> findChildIds(long? catalogId);
    ActionPlanCatalog findActionPlanCatalogByParentId(long? childId);
    void deleteByMainActionPlanCatalogId(long? mainActionPlanCatalogId);
    bool existsByRelatedActionPlanCatalogId(long? relatedActionPlanCatalogId);
    // @Query("SELECT r.relatedActionPlanCatalog.id FROM ActionPlanCatalogRelation r WHERE r.mainActionPlanCatalog.id = :mainCatalogId")
    List<long?> findRelatedCatalogIdsByMainCatalogId(long? mainCatalogId);
}
