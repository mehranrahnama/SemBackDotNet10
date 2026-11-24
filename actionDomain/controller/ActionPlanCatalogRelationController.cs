// Mechanical conversion from Java (controller/ActionPlanCatalogRelationController.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.basicDomain.Repositories;



namespace SemBackDotNet10.actionDomain.controller;

/// <summary>معادل ActionPlanCatalogRelationController.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class ActionPlanCatalogRelationController
{
    private readonly IActionPlanCatalogRelationService _actionPlanCatalogRelationService;

    public ActionPlanCatalogRelationController(IActionPlanCatalogRelationService actionPlanCatalogRelationService)
    {
        _actionPlanCatalogRelationService = actionPlanCatalogRelationService;
    }

    // TODO(signature): public ResponseEntity<?> create(@RequestHeader Map<String,String> headers, actionPlanCatalogRelationService.create(actionPlanCatalogRelationDtos); return new Re
    // TODO(signature): ) { SearchDto data = actionPlanCatalogRelationService.getAllRelatedActionPlanCatalog(requestDto, page, size, orderBy, direction); return new ResponseEntity<>(da
    [HttpGet("relatedActionPlanCatalog/{mainActionPlanCatalogId}")]
    IActionResult getAllRelatedActionPlanCatalog(Dictionary<string?,string?> headers, long? mainActionPlanCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — mainActionPlanCatalogId)
        throw new NotImplementedException();
    }
    [HttpDelete("actionPlanCatalogRelation/{id}")]
    IActionResult deleteActionPlanCatalogRelation(Dictionary<string?,string?> headers, long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // TODO(signature): public ResponseEntity<?> updateWeight(@RequestHeader Map<String,String> headers, actionPlanCatalogRelationService.updateWeight(updateActionPlanCatalogDtos); ret
}
