// Mechanical conversion from Java (controller/MeasureCatalogRelationController.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureCatalogRelationController.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureCatalogRelationController
{
    // TODO(signature): SearchDto userPage = measureCatalogRelationService.findByMainCatalogId(mainRelId,filter, page, size, orderBy, direction)
    // TODO(signature): return new ResponseEntity<>(userPage, HttpStatus.OK)
    IActionResult editMeasureCatalogRelation(Dictionary<string?,string?> headers, List<MeasureCatalogRelationDto> measureCatalogRelationDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureCatalogRelationDto)
        throw new NotImplementedException();
    }
    // TODO(signature): 
    [HttpDelete("measureCatalogRelation/{id}")]
    IActionResult deleteMeasureCatalogRelation(Dictionary<string?,string?> headers, long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // TODO(signature): ){ measureCatalogRelationService.deleteRelationsForMeasureCatalog(catalogId); return new ResponseEntity<>(HttpStatus.OK); }
    [HttpPost("measureCatalogRelation")]
    IActionResult createMeasureCatalogRelation(Dictionary<string?,string?> headers, MeasureCatalogRelationDto measureCatalogRelationDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureCatalogRelationDto)
        throw new NotImplementedException();
    }
}
