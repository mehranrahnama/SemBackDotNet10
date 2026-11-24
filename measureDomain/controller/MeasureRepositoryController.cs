// Mechanical conversion from Java (controller/MeasureRepositoryController.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureRepositoryController.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureRepositoryController
{
    private readonly IMeasureRepositoryService _measureRepositoryService;

    public MeasureRepositoryController(IMeasureRepositoryService measureRepositoryService)
    {
        _measureRepositoryService = measureRepositoryService;
    }

    // TODO(ctor): سازنده‌ی جاوا (جز تزریق وابستگی) منتقل نشده است.
    // TODO(signature): ) { this.measureRepositoryService = measureRepositoryService; }
    // TODO(signature): SearchDto userPage = measureRepositoryService.findAll(filter, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(userPage, HttpStatus.OK)
    //----------get measure unit by repo id---------
    [HttpGet("measureRepository/getMeasureUnit/{repositoryId}")]
    IActionResult getMeasureUnit(Dictionary<string?,string?> headers, long? repositoryId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — repositoryId)
        throw new NotImplementedException();
    }
    [HttpGet("measureRepository/findMeasureRepositoryById/{repositoryId}")]
    IActionResult findMeasureRepositoryById(Dictionary<string?,string?> headers, long? repositoryId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — repositoryId)
        throw new NotImplementedException();
    }
    [HttpGet("measureRepository/findMeasureRepositoryByMeasureCatalogId/{measureCatalogId}/{measureConfirmStatus}")]
    IActionResult findMeasureRepositoryByMeasureCatalogId(Dictionary<string?,string?> headers, long? measureCatalogId, long? measureConfirmStatus)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureConfirmStatus)
        throw new NotImplementedException();
    }
    [HttpGet("measureRepository/findMeasureRepositoryInfoByMeasureCatalogId/{measureCatalogId}")]
    IActionResult findMeasureRepositoryInfoByMeasureCatalogId(Dictionary<string?,string?> headers, long? measureCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureCatalogId)
        throw new NotImplementedException();
    }
    //    @GetMapping(value = "measureRepository/findMeasureCatalogInfoStatusLogByMeasureCatalogId/{measureCatalogId}")
    //    public ResponseEntity<?> findMeasureInfoWithStatusLogByMeasureCatalogId(@PathVariable Long measureCatalogId){
    //        MeasureRepositoryDto measureRepositoryDto=measureRepositoryService.findMeasureCatalogInfoStatusLogByMeasureCatalogId(measureCatalogId);
    //        return new ResponseEntity<>(measureRepositoryDto,HttpStatus.OK);
    //    }
    [HttpDelete("measureRepository/{id}")]
    IActionResult deleteMeasureRepository(Dictionary<string?,string?> headers, long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    [HttpPost("measureRepository")]
    IActionResult createMeasureRepository(global::Microsoft.AspNetCore.Http.HttpRequest request, Dictionary<string?,string?> headers, MeasureRepositoryDto measureRepositoryDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureRepositoryDto)
        throw new NotImplementedException();
    }
    [HttpPut("measureRepository")]
    IActionResult editMeasureRepository(Dictionary<string?,string?> headers, MeasureRepositoryDto measureRepositoryDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureRepositoryDto)
        throw new NotImplementedException();
    }
    // TODO(signature): public ResponseEntity<?> findByIdMeasureRepository(@RequestHeader Map<String,String> headers, MeasureRepositoryDto measureRepositoryDto = measureRepositoryServi
    // TODO(signature): public ResponseEntity<?> getAllMeasureRepositories_reportDataSet(@RequestHeader Map<String,String> headers, String dtos = measureRepositoryService.getAllMeasure
    // TODO(signature): public ResponseEntity<?> hasMeasureActPlan(@RequestHeader Map<String,String> headers, boolean result =  measureRepositoryService.hasMeasureActPlan(measureReposi
}
