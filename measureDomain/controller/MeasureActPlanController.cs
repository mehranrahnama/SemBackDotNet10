// Mechanical conversion from Java (controller/MeasureActPlanController.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.measureDomain.model.entity;
using SemBackDotNet10.measureDomain.model.dto;
using SemBackDotNet10.measureDomain.controller.dto;
using SemBackDotNet10.measureDomain.mapper;
using SemBackDotNet10.measureDomain.repository;
using SemBackDotNet10.measureDomain.service.interf;
using SemBackDotNet10.measureDomain.domain;
using SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.context;
using SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.factory;
using SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.handler;
using global::Common.model.entity;
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.baseDomain.repository;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;
using global::Common.repository;
using global::Common.service.interf;
using global::Common.model.dto;
using global::Common.controller.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;
using global::Common.messages;
using global::Common.repository;
using global::Common.service.interf;
using global::Common.service.impl;


namespace SemBackDotNet10.measureDomain.controller;

/// <summary>معادل MeasureActPlanController.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureActPlanController
{
    private readonly IMeasureActPlanService _measureActPlanService;
    private readonly IPCNCalculationFormulaService _pcnCalculationFormulaService;

    public MeasureActPlanController(IMeasureActPlanService measureActPlanService, IPCNCalculationFormulaService pcnCalculationFormulaService)
    {
        _measureActPlanService = measureActPlanService;
        _pcnCalculationFormulaService = pcnCalculationFormulaService;
    }

    // TODO(ctor): سازنده‌ی جاوا (جز تزریق وابستگی) منتقل نشده است.
    // TODO(signature): IPCNCalculationFormulaService pcnCalculationFormulaService ) { this.measureActPlanService = measureActPlanService; this.pcnCalculationFormulaService = pcnCalcula
    // TODO(signature): SearchDto userPage = measureActPlanService.insertEmptyMeasureActPlan(requestDto, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(userPage, HttpStatus.OK)
    // TODO(signature): SearchDto userPage = measureActPlanService.createMeasureActPlan(measureActPlanRequestDto, page, size, orderBy, direction,false)
    // TODO(signature): return new ResponseEntity<>(userPage, HttpStatus.OK)
    // TODO(signature): SearchDto userPage = measureActPlanService.getCustomMeasureActPlan(body, page, size, orderBy, direction, false)
    // TODO(signature): return new ResponseEntity<>(userPage, HttpStatus.OK)
    // TODO(signature): measureActPlanService.MeasureActRePlan(requestDto, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(HttpStatus.OK)
    // TODO(signature): ) { measureActPlanService.newPlanningByMeasure(dto); return new ResponseEntity<>(HttpStatus.OK); }
    // TODO(signature): ) throws Exception { measureActPlanService.saveValueMeasureActReplan(requestInfo.getParam1() , requestInfo.getParam2()); return new ResponseEntity<>(HttpStatus.
    // TODO(signature): ) throws Exception { MeasureWrapperDTO userPage = measureActPlanService.getMeasureActPlanByMeasureId(measureId, page, size, orderBy, direction); return new Resp
    // TODO(signature): ) throws Exception { MeasureWrapperDTO userPage = measureActPlanService.getCustomMeasureActPlanByMeasureId(measureId, page, size, orderBy, direction); return ne
    [HttpGet("findAllmeasureActPlansByMeasureId/{measureId}")]
    IActionResult findAllmeasureActPlansByMeasureId(Dictionary<string?,string?> headers, long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    [HttpGet("findAllmeasureActPlansOfMeasure/{measureActPlanId}")]
    IActionResult findAllmeasureActPlansOfMeasure(Dictionary<string?,string?> headers, long? measureActPlanId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlanId)
        throw new NotImplementedException();
    }
    [HttpGet("findMeasureRepositoryInfoByMeasureActPlanId/{measureActPlanId}")]
    IActionResult findMeasureRepositoryInfoByMeasureActPlanId(Dictionary<string?,string?> headers, long? measureActPlanId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlanId)
        throw new NotImplementedException();
    }
    //findAllmeasureActPlansByMeasureId
    [HttpPut("measureActPlan")]
    IActionResult getControlPanelByEntity(Dictionary<string?,string?> headers, MeasureActPlanMeasureDto paramMapList)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — paramMapList)
        throw new NotImplementedException();
    }
    // TODO(signature): public ResponseEntity<?> MeasureActPlanFormula(@RequestHeader Map<String,String> headers, measureActPlanService.parseCalculateAchievedPCNFormula(body); SearchDt
    [HttpPost("measureActPlan/saveFormula")]
    IActionResult createPCNCalculationFormula(Dictionary<string?,string?> headers, PCNCalculationFormulaDto pCNCalculationFormulaDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — pCNCalculationFormulaDto)
        throw new NotImplementedException();
    }
    // TODO(signature): SearchDto userPage = pcnCalculationFormulaService.findAll(filter, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(userPage, HttpStatus.OK)
    [HttpPost("measureActPlan/calculatePCNForAllMeasureActPlans")]
    IActionResult calculatePCNForAllMeasureActPlans(Dictionary<string?,string?> headers, string? filter)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — filter)
        throw new NotImplementedException();
    }
    // TODO(signature): SearchDto searchDto = measureActPlanService.loadMeasureProgressGrid( headers, body, page, size, orderBy, direction )
    // TODO(signature): return new ResponseEntity<>(searchDto, HttpStatus.OK)
    [HttpPut("measureActPlan/evolution-Committee-values")]
    IActionResult calculateActualAndDailyValues(Dictionary<string?,string?> headers, string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    //----------edit daily values---------
    [HttpPut("measureActPlan/daily-value")]
    IActionResult editDailyValues(Dictionary<string?,string?> headers, List<MeasureProgressDto> measureProgressDtos)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureProgressDtos)
        throw new NotImplementedException();
    }
    [HttpPut("measureActPlan/taskUrl")]
    IActionResult setTaskUrl(Dictionary<string?,string?> headers, List<MeasureProgressDto> measureProgressDtos)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureProgressDtos)
        throw new NotImplementedException();
    }
    //----------edit deviation---------
    [HttpPut("measureActPlan/deviation")]
    IActionResult editDeviation(Dictionary<string?,string?> headers, MeasureProgressDto measureProgressDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureProgressDto)
        throw new NotImplementedException();
    }
    //----------edit correction---------
    [HttpPut("measureActPlan/correction")]
    IActionResult editCorrection(Dictionary<string?,string?> headers, MeasureProgressDto measureProgressDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureProgressDto)
        throw new NotImplementedException();
    }
    [HttpGet("measureActPlan/correctionByMeasureId/{id}")]
    IActionResult correctionByMeasureId(Dictionary<string?,string?> headers, long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // TODO(signature): public ResponseEntity<?> editStatus(@RequestHeader Map<String,String> headers, measureActPlanService.editStatus(measureProgressDto); return new ResponseEntity<>
    //----------check access level of user in measure progress form---------
    //    @PostMapping("measureActPlan/access-level")
    //    public ResponseEntity<?> hasAccessLevel(@RequestBody String body) {
    //        return new ResponseEntity<>(measureActPlanService.hasAccessLevel(body), HttpStatus.OK);
    //    }
    //    @Scheduled(cron = "0 0 0 * * *", zone = "Asia/Tehran")
    [HttpGet("measureActPlan/expireMeasureActPlanTasks")]
    IActionResult expireMeasureActPlanTasks(Dictionary<string?,string?> headers)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — headers)
        throw new NotImplementedException();
    }
    // TODO(signature): public ResponseEntity<?> getCorrectionActualValueForHOSCO(@RequestHeader Map<String,String> headers, BigDecimal numProgressAction =   measureActPlanService.getC
    // TODO(signature): public ResponseEntity<?> deleteMeasureActPlan(@RequestHeader Map<String,String> headers, measureActPlanService.deleteMeasureActPlan(measureId); return new Respo
    // TODO(signature): public ResponseEntity<?> calcMeasurePCN(@RequestHeader Map<String,String> headers, BigDecimal MeasurePCN =   measureActPlanService.calcMeasurePCN(measureActPlan
}
