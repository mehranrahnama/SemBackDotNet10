using StrategyObjectService = global::SemBackDotNet10.basicDomain.service.IStrategyObjectService;
using Measure = global::SemBackDotNet10.measureDomain.model.entity.Measure;
using MeasureRepository = global::SemBackDotNet10.measureDomain.model.entity.MeasureRepository;
// Mechanical conversion from Java (service/impl/MeasureStrategyPropagationServiceImpl.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureStrategyPropagationServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureStrategyPropagationServiceImpl : IMeasureStrategyPropagationService
{
    private readonly IMeasureCatalogRelationService _measureCatalogRelationService;
    private readonly global::SemBackDotNet10.baseDomain.resolver.IStrategyObjectResolver _strategyObjectResolver;
    private readonly MeasureRepository _measureRepository;
    private readonly StrategyObjectService _strategyObjectService;

    public MeasureStrategyPropagationServiceImpl(IMeasureCatalogRelationService measureCatalogRelationService, global::SemBackDotNet10.baseDomain.resolver.IStrategyObjectResolver strategyObjectResolver, MeasureRepository measureRepository, StrategyObjectService strategyObjectService)
    {
        _measureCatalogRelationService = measureCatalogRelationService;
        _strategyObjectResolver = strategyObjectResolver;
        _measureRepository = measureRepository;
        _strategyObjectService = strategyObjectService;
    }

    // @Transactional(propagation = Propagation.MANDATORY)
    public void propagateStrategyObjectChange(Measure rootMeasure, long? upperLevelStrategyObjectId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — upperLevelStrategyObjectId)
        throw new NotImplementedException();
    }
}
