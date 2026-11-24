using global::SemBackDotNet10.Base.Service.Inter;
// Mechanical conversion from Java (service/interf/IMeasureCatalogStatusLogService.java). Complex bodies are marked with TODO.
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



namespace SemBackDotNet10.measureDomain.service.interf;

/// <summary>معادل IMeasureCatalogStatusLogService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IMeasureCatalogStatusLogService : IBaseService<MeasureCatalogStatusLog,long?,MeasureCatalogStatusLogRepository>
{
    MeasureCatalogStatusLogDto findMeasureCatalogStatusLogByParams(long? measureCatalogId, long? measureConfirmStatus);
    MeasureCatalogStatusLog create(MeasureCatalogStatusLogDto measureCatalogStatusLogDto);
}
