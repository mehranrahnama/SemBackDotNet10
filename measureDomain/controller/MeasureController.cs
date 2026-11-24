using global::SemBackDotNet10.measureDomain.controller.dto.measureDtos;
// Mechanical conversion from Java (controller/MeasureController.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureController.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureController
{
    private readonly IMeasureService _measureService;

    public MeasureController(IMeasureService measureService)
    {
        _measureService = measureService;
    }

    // TODO(signature): ) { SearchDto searchDto = measureService.loadGridByObjectId(body, page, size, orderBy, direction); return new ResponseEntity<>(searchDto, HttpStatus.OK); }
    // TODO(signature): SearchDto searchDto = measureService.loadGridByObjectIdAndIsPlanned(body,page,size,orderBy,direction)
    // TODO(signature): return new ResponseEntity<>(searchDto, HttpStatus.OK)
    //----------find by id---------
    [HttpGet("measure/{id}")]
    IActionResult findMeasureById(Dictionary<string?,string?> headers, long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    [HttpGet("measureByMeasureCatalogId/{PlanId}/{MeasureCatalogId}")]
    IActionResult findMeasureByMeasureCatalogId(Dictionary<string?,string?> headers, long? PlanId, long? MeasureCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — MeasureCatalogId)
        throw new NotImplementedException();
    }
    //----------save---------
    [HttpPost("measure")]
    IActionResult create(Dictionary<string?,string?> headers, RequestMeasureDto measureDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureDto)
        throw new NotImplementedException();
    }
    // TODO(signature): ) throws Exception { Measure measure = measureService.edit(measureDto); MeasureDto result = measureService.mapToDto(measure); return new ResponseEntity<>(result
    //----------edit all weights---------
    [HttpPut("measure/weights")]
    IActionResult editAllWeights(Dictionary<string?,string?> headers, List<MeasureDto> measureDtos)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureDtos)
        throw new NotImplementedException();
    }
    [HttpPut("editIsReplannedMeasure/{MeasureCatalogId}")]
    IActionResult editIsReplannedMeasure(Dictionary<string?,string?> headers, long? MeasureCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — MeasureCatalogId)
        throw new NotImplementedException();
    }
    //----------delete---------
    [HttpDelete("measure/{id}")]
    IActionResult delete(Dictionary<string?,string?> headers, long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { SearchDto searchDto = measureService.loadObjectGrid(body, page, size, orderBy, direction); return new ResponseEntity<>(searchDto, HttpStatus.OK); }
    //----------edit Delete Status---------
    [HttpPut("measure/delete-status")]
    IActionResult editDeleteStatus(Dictionary<string?,string?> headers, string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    // TODO(signature): public ResponseEntity<?> getCounts(@RequestHeader Map<String,String> headers, Map<String,Object> counts = measureService.getCounts(body); return new ResponseEnt
    // TODO(signature): ){ measureService.editConfirmStatus(id, outCome); return new ResponseEntity<>(HttpStatus.OK); }
    // TODO(signature): public ResponseEntity<?> CountActiveMeasures(@RequestHeader Map<String,String> headers, Long measureCount=measureService.CountActiveMeasures(planId); return new
    [HttpGet("measure/expireMeasureTasks")]
    IActionResult expireMeasureTasks(Dictionary<string?,string?> headers)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — headers)
        throw new NotImplementedException();
    }
    // TODO(signature): public ResponseEntity<?> measureReplanConfirm(@RequestHeader Map<String,String> headers, Measure measure = measureService.measureReplanConfirm(measureDto); retu
    // TODO(signature): ) { measureService.measureReplanReject(measureId); return new ResponseEntity<>(HttpStatus.OK); }
}
