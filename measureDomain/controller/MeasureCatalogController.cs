// Mechanical conversion from Java (controller/MeasureCatalogController.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.measureDomain.service.impl;
using SemBackDotNet10.Controllers;
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

/// <summary>معادل MeasureCatalogController.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
[ApiController]
[Route("api/v1")]
public class MeasureCatalogController : ControllerBase
{
    private readonly HamsooMeasureService _hamsooMeasureService;
    private readonly IMeasureCatalogService _measureCatalogService;

    public MeasureCatalogController(HamsooMeasureService hamsooMeasureService, IMeasureCatalogService measureCatalogService)
    {
        _hamsooMeasureService = hamsooMeasureService;
        _measureCatalogService = measureCatalogService;
    }

    // TODO(ctor): سازنده‌ی جاوا (جز تزریق وابستگی) منتقل نشده است.
    // TODO(signature): HamsooMeasureService hamsooMeasureService ) { this.measureCatalogService = measureCatalogService; this.hamsooMeasureService = hamsooMeasureService; }
    // TODO(signature): ){ measureCatalogService.UpdateMeasureConfirmStatus(id, outCome); return new ResponseEntity<>(HttpStatus.OK); }
    // TODO(signature): ) throws Exception { try { measureCatalogService.deleteMeasureCatalog(id,request); } catch (DataIntegrityViolationException e) { throw new BusinessException(e.g
    // TODO(signature): ) throws Exception { Long tenantId= Long.valueOf(headers.get("factoryid")); boolean result = measureCatalogService.expireMeasureCatalogTasks(tenantId); if (resu
    [HttpGet("measureCatalog/insertHamsooMeasure")]
    [CheckAccess]
    public async Task<IActionResult> InsertHamsooMeasure(CancellationToken ct)
    {
        await _hamsooMeasureService.ImportProcessMeasuresAsync(ct);
        return Ok();
    }
    // TODO(signature): ){ SearchDto result = measureCatalogService.getMeasureCatalogModir( body,levelId , page, size, orderBy, direction); return new ResponseEntity<>(result , HttpSta
    // TODO(signature): ) { SearchDto data = measureCatalogService.findAllMeasureCatalog(requestDto); return new ResponseEntity<>(data, HttpStatus.OK); }
}
