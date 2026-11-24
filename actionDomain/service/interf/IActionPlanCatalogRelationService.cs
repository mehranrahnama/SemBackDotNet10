using global::SemBackDotNet10.Base.Service.Inter;
using global::SemBackDotNet10.actionDomain.controller.dto.ActionPlanCatalogRelationDtos;
using global::SemBackDotNet10.actionDomain.model.dto.actionPlanCatalogRelationDtos;
// Mechanical conversion from Java (service/interf/IActionPlanCatalogRelationService.java). Complex bodies are marked with TODO.
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


namespace SemBackDotNet10.actionDomain.service.interf;

/// <summary>معادل IActionPlanCatalogRelationService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IActionPlanCatalogRelationService : IBaseService<ActionPlanCatalogRelation,long?,ActionPlanCatalogRelationRepository>
{
    ActionPlanCatalogRelation findByIdOrElseThrow(long? id);
    // TODO(signature): ) throws Exception;
    // TODO(signature): List<PlaningLevel> planingLevelList );
    void deleteByRelatedCatalogId(long? relatedCatalogId);
    void deleteByMainCatalogId(long? mainCatalogId);
    List<long?> findAncestorIds(long? leafId);
    List<ActionPlanCatalog> findAncestorEntity(long? leafId);
    // TODO(signature): SortDirection direction);
    void delete(long? id);
    void updateWeight(List<UpdateActionPlanCatalogRelationDto> updateActionPlanCatalogDtos);
    List<RelatedDto> getAllRelatedActionPlanCatalogByMainId(long? mainActionPlanCatalogId);
    HashSet<ActionPlanCatalog> findConnectedActionPlanCatalogIds(long? catalogId);
}
