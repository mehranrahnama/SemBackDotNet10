// Mechanical conversion from Java (controller/ActionActPlanController.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System.Collections.Generic;
using Common.utils;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.service.interf;

namespace SemBackDotNet10.actionDomain.controller;

/// <summary>معادل ActionActPlanController.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class ActionActPlanController : ControllerBase
{
    private readonly IActionActPlanService _actionActPlanService;
    private readonly IActionPlanActivityService _actionPlanActivityService;

    public ActionActPlanController(IActionActPlanService actionActPlanService, IActionPlanActivityService actionPlanActivityService)
    {
        _actionActPlanService = actionActPlanService;
        _actionPlanActivityService = actionPlanActivityService;
    }

    // TODO(ctor): سازنده‌ی جاوا (جز تزریق وابستگی) منتقل نشده است.
    // TODO(signature): IActionPlanActivityService actionPlanActivityService ) { this.actionActPlanService = actionActPlanService; this.actionPlanActivityService = actionPlanActivitySer
    // TODO(signature): ) { SearchDto userPage = actionActPlanService.CreateAndGetDataActionActPlan(body, page, size, orderBy, direction); return new ResponseEntity<>(userPage, HttpSta
    // TODO(signature): ) { return new ResponseEntity<>(actionActPlanService.getActionActPlan(actionPlanCatalogId), HttpStatus.OK); }
    // TODO(signature): ) { actionActPlanService.updatePlanValues(actionActPlanDataDtos, actionPlanCatalogId, strategyObjectId); return ResponseEntity.ok().build(); }
    // TODO(signature): ) { actionPlanActivityService.addActivityFromProgress(actionActPlanId, activity); return ResponseEntity.ok().build(); }
    // TODO(signature): ) { actionActPlanService.removeDefaultPlanning(actionPlanCatalogId); return ResponseEntity.ok().build(); }
    // TODO(signature): public ResponseEntity<?> calculatePlanValue( actionActPlanService.calculationDefaultPlanValue(body); return new ResponseEntity<>(HttpStatus.OK); }
    // TODO(signature): SearchDto userPage = actionActPlanService.findAllActionActPlan(body, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(userPage, HttpStatus.OK)
    // TODO(signature:): SearchDto userPage = actionActPlanService.CreateOrGetDataActionActPlan(body, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(userPage, HttpStatus.OK)
    // TODO(signature): SearchDto searchDto = actionActPlanService.loadGrid(headers,body, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(searchDto, HttpStatus.OK)
    // TODO(signature): SearchDto searchDto = actionActPlanService.loadGrid(headers,body, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(searchDto, HttpStatus.OK)
    //    @GetMapping(value = "actionActPlan/{id}")
    //    public ResponseEntity<?> getById(@PathVariable("id") Long id) {
    //        ActionProgressDto searchDto = actionActPlanService.findByActionId(id);
    //        if (searchDto == null)
    //            return new ResponseEntity<>(HttpStatus.NO_CONTENT);
    //        return new ResponseEntity<>(searchDto, HttpStatus.OK);
    //
    //    }
    [HttpPost("actionActPlan/details")]
    public IActionResult getActionActPlan(Dictionary<string?, string?> headers, string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    // TODO(signature): orderBy = "h
    // TODO(signature): SearchDto searchDto = actionActPlanService.relatedValueGrid(body, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(searchDto, HttpStatus.OK)
    // TODO(signature): ) { actionActPlanService.EditActionActPlan(actPlanDtoList); return new ResponseEntity<>(HttpStatus.OK); }
    //    @PutMapping("actionActPlan/calculationPlanValue")
    //    public ResponseEntity<?> calculationPlanValue(@RequestParam int page, @RequestParam int size,
    //                                                  @RequestParam(required = false) SortDirection direction,
    //                                                  @RequestParam(required = false) String orderBy,
    //                                                  @RequestParam("actionPlanCatalogId") Long actionPlanCatalogId,
    //                                                  @RequestParam("levelId") Long levelId
    //    )
    //    {
    //        SearchDto data= actionActPlanService.calculationPlanValue( actionPlanCatalogId ,  levelId);
    //        return new ResponseEntity<>(data,HttpStatus.OK);
    //    }
    //----------edit deviation---------
    [HttpPut("actionActPlan/deviation")]
    public IActionResult editDeviation(Dictionary<string?, string?> headers, ActionActPlanDto actionActPlanDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionActPlanDto)
        throw new NotImplementedException();
    }
    //----------edit correction---------
    [HttpPut("actionActPlan/correction")]
    public IActionResult editCorrection(Dictionary<string?, string?> headers, ActionActPlanDto actionActPlanDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionActPlanDto)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { actionActPlanService.editStatus(actionActPlanDto); return new ResponseEntity<>(HttpStatus.OK); }
    //----------edit Actual Value And Budge---------
    [HttpPut("actionActPlan/value-budget")]
    public IActionResult editAllActualValueAndBudget(Dictionary<string?, string?> headers, List<ActionActPlanDto> actionActPlanDtoList)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionActPlanDtoList)
        throw new NotImplementedException();
    }
    [HttpPut("actionActPlan/taskUrl")]
    public IActionResult setTaskUrl(Dictionary<string?, string?> headers, List<ActionActPlanDto> actionActPlanDtoList)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionActPlanDtoList)
        throw new NotImplementedException();
    }
    //----------extend EndDate Of Expired Project (schedule)---------
    [HttpGet("actionActPlan/extend-date")]
    public IActionResult extendEndDateOfExpiredProject(Dictionary<string?, string?> headers)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — headers)
        throw new NotImplementedException();
    }
    //----------extend EndDate Of Expired Project (load of form)---------
    [HttpPost("actionActPlan/extend-date")]
    public IActionResult extendEndDateOfExpiredProjectByFilter(Dictionary<string?, string?> headers, string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    [HttpPut("actionActPlan/changeActionActPlanSchedule")]
    public IActionResult changeActionActPlanSchedule(Dictionary<string?, string?> headers, List<ActionActPlanDto> actionActPlanDtoList)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionActPlanDtoList)
        throw new NotImplementedException();
    }
    // TODO(signature): SearchDto searchDto = actionActPlanService.loadReplanGrid(body, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(searchDto, HttpStatus.OK)
    // TODO(signature): ) { return new ResponseEntity<>( actionActPlanService.loadReplanDataByActionPlanCatalog(actionPlanCatalogId), HttpStatus.OK ); }
    // TODO(signature): ) { actionActPlanService.replanActionPlanCatalogAndSendToCartable(actionPlanCatalogId, strategyObjectId, dto); return new ResponseEntity<>(HttpStatus.OK); }
    // TODO(signature): SearchDto searchDto = actionActPlanService.loadReplanCartableGrid(body, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(searchDto, HttpStatus.OK)
    //----------save replan values---------
    [HttpPut("actionActPlan/replan")]
    public IActionResult saveReplanValues(
        Dictionary<string?, string?> headers,
        RequestInfoTwoParam<List<ActionActReplanDto>, Dictionary<string, string>> requestInfo)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — requestInfo)
        throw new NotImplementedException();
    }
    [HttpGet("actionActPlan/correctionByActPlanId/{id}")]
    public IActionResult correctionByActPlanId(Dictionary<string?, string?> headers, long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    [HttpGet("findAllActionActPlansOfActionPlan/{actionActPlanId}")]
    public IActionResult findAllActionActPlansOfMeasure(Dictionary<string?, string?> headers, long? actionActPlanId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionActPlanId)
        throw new NotImplementedException();
    }
    // TODO(signature): public ResponseEntity<?> deleteActionActPlanByActionPlanCatalog(@RequestHeader Map<String,String> headers, actionActPlanService.deleteActionActPlanByActionPlanC
    [HttpGet("actionActPlan/getCorrectionActualValueForHOSCO/{actionActPlanId}")]
    public IActionResult getCorrectionActualValueForHOSCO(Dictionary<string?, string?> headers, long? actionActPlanId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionActPlanId)
        throw new NotImplementedException();
    }
    // TODO(signature): public ResponseEntity<?> deleteActionActPlan(@RequestHeader Map<String,String> headers, actionActPlanService.deleteActionPlanCatalog(actionPlanCatalogId); retur
}
