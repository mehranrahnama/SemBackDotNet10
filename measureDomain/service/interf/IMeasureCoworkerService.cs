// Mechanical conversion from Java (service/interf/IMeasureCoworkerService.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.Base.Service.Inter;



namespace SemBackDotNet10.measureDomain.service.interf;

/// <summary>معادل IMeasureCoworkerService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IMeasureCoworkerService : IBaseService<MeasureCoworker,long?,MeasureCoworkerRepository>
{
    void createMeasureCoworker(Measure measure, List<long?> planingLevelOrgIds);
}
