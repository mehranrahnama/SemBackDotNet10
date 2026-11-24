// Mechanical conversion from Java (controller/ActionPlanController.java). Complex bodies are marked with TODO.
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



namespace SemBackDotNet10.actionDomain.controller;

/// <summary>معادل ActionPlanController.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class ActionPlanController
{
    private readonly IActionPlanService _actionPlanService;

    public ActionPlanController(IActionPlanService actionPlanService)
    {
        _actionPlanService = actionPlanService;
    }

    // TODO(signature): SearchDto searchDto = actionPlanService.loadGrid(body, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(searchDto, HttpStatus.OK)
    // TODO(signature): ) { return new ResponseEntity<>( actionPlanService.loadGridByObjectId(body, page, size, orderBy, direction), HttpStatus.OK ); }
    // TODO(signature): SearchDto searchDto = actionPlanService.loadGridByObjectIdAndIsPlanned(body, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(searchDto, HttpStatus.OK)
    // TODO(signature): ) { actionPlanService.create_V2( requestInfo.getParam1(), requestInfo.getParam2(), UpperLevelsSendMode.of(sendToUpperLevels, sendToParentOnly) ); return new Res
    // TODO(signature): ) throws Exception { actionPlanService.editWithActivity( requestInfo.getParam1(), requestInfo.getParam2(), UpperLevelsSendMode.of(sendToUpperLevels, sendToParen
    // TODO(signature): ) { actionPlanService.editConfirmStatus(id, outCome); return new ResponseEntity<>(HttpStatus.OK); }
    // TODO(signature): ) throws Exception { actionPlanService.editConfirmStatusForDelete(actionPlan); return new ResponseEntity<>(HttpStatus.OK); }
    //----------edit replanStatus---------
    //----------edit Delete Status---------
    [HttpPut("v1/actionPlan/delete-status")]
    IActionResult editDeleteStatus(Dictionary<string?,string?> headers, string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    //----------delete---------
    [HttpDelete("v1/actionPlan/{id}")]
    IActionResult delete(Dictionary<string?,string?> headers, long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { return new ResponseEntity<>( actionPlanService.loadObjectGrid(body, page, size, orderBy, direction), HttpStatus.OK ); }
    //----------edit all weights---------
    [HttpPut("v1/actionPlan/weights")]
    IActionResult editAllWeights(Dictionary<string?,string?> headers, List<ActionPlanDto> actionPlanDtos)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanDtos)
        throw new NotImplementedException();
    }
    // TODO(signature): public ResponseEntity<?> getCounts(@RequestHeader Map<String,String> headers, Map<String,Object> counts = actionPlanService.getCounts(body); return new Response
    //----------check access level of user in action management form---------
    //    @PostMapping("v1/actionPlan/access-level")
    //    public ResponseEntity<?> hasAccessLevel(@RequestBody(required = false) String body) {
    //        return new ResponseEntity<>(actionPlanService.hasAccessLevel(body), HttpStatus.OK);
    //    }
    //----------find by id---------
    [HttpGet("v1/actionPlan/{id}")]
    IActionResult findActionPlanById(Dictionary<string?,string?> headers, long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // TODO(signature): public ResponseEntity<?> CountActiveMeasures(@RequestHeader Map<String,String> headers, Long actionsCount=actionPlanService.CountActiveActions(planId); return n
    [HttpGet("v1/actionPlan/expireActionPlanTasks")]
    IActionResult expireActionPlanTasks(Dictionary<string?,string?> headers)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — headers)
        throw new NotImplementedException();
    }
    [HttpPut("v1/deleteActionPlan/{actionPlanCatalogId}")]
    IActionResult deleteActionPlanByActionPlanCatalogId(Dictionary<string?,string?> headers, long? actionPlanCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanCatalogId)
        throw new NotImplementedException();
    }
}
