using Measure = global::SemBackDotNet10.measureDomain.model.entity.Measure;
using MeasureRepository = global::SemBackDotNet10.measureDomain.model.entity.MeasureRepository;
// Mechanical conversion from Java (domain/MeasureDomainService.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.basicDomain.Repositories;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Infrastructure.Persistence;
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


namespace SemBackDotNet10.measureDomain.domain;

/// <summary>معادل MeasureDomainService.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureDomainService
{
    private readonly MeasureRepository _repository;

    public MeasureDomainService(MeasureRepository repository)
    {
        _repository = repository;
    }

    List<long?> findMeasureIdsByMeasureCatalogId(List<long?> ancestorIds)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — ancestorIds)
        throw new NotImplementedException();
    }
    List<Measure> findByCatalogIds(List<long?> catalogIds)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogIds)
        throw new NotImplementedException();
    }
    void save(Measure entity)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — entity)
        throw new NotImplementedException();
    }
    void saveAll(List<Measure> entities)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — entities)
        throw new NotImplementedException();
    }
    string? generateTaskUrl(long? id, long? newStatus, long? oldStatus)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — oldStatus)
        throw new NotImplementedException();
    }
    void deleteById(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    Measure getByIdOrThrow(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    bool checkIsNotPlannedById(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    void restoreByCatalogId(long? catalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogId)
        throw new NotImplementedException();
    }
}
