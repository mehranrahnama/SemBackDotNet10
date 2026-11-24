using Measure = global::SemBackDotNet10.measureDomain.model.entity.Measure;
// Mechanical conversion from Java (service/impl/MeasureCatalogLogServiceImpl.java). Complex bodies are marked with TODO.
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


namespace SemBackDotNet10.measureDomain.service.impl;

/// <summary>معادل MeasureCatalogLogServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureCatalogLogServiceImpl : IMeasureCatalogLogService
{
    private readonly MeasureCatalogLogRepository _measureCatalogLogRepository;
    private readonly IMeasureService _measureService;
    private readonly global::SemBackDotNet10.basicDomain.service.IUserService _userService;

    public MeasureCatalogLogServiceImpl(MeasureCatalogLogRepository measureCatalogLogRepository, IMeasureService measureService, global::SemBackDotNet10.basicDomain.service.IUserService userService)
    {
        _measureCatalogLogRepository = measureCatalogLogRepository;
        _measureService = measureService;
        _userService = userService;
    }

    // TODO(ctor): سازنده‌ی جاوا (جز تزریق وابستگی) منتقل نشده است.
    // TODO(signature): this.measureCatalogLogRepository = measureCatalogLogRepository
    public MeasureCatalogLogRepository GetRepository() => _measureCatalogLogRepository;

    public MeasureCatalogLogRepository getRepository() => GetRepository();
    public MeasureCatalogLog createMeasureCatalogLog(Measure measure, long? newTashilgarId, long? newModirId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — newModirId)
        throw new NotImplementedException();
    }
    public MeasureCatalogLog createMeasureCatalogLog(MeasureCatalog measureCatalog, long? newTashilgarId, long? newModirId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — newModirId)
        throw new NotImplementedException();
    }
}
