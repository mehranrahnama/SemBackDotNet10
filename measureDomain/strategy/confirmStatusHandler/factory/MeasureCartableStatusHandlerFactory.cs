// Mechanical conversion from Java (strategy/confirmStatusHandler/factory/MeasureCartableStatusHandlerFactory.java). Complex bodies are marked with TODO.
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



namespace SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.factory;

/// <summary>معادل MeasureCartableStatusHandlerFactory.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureCartableStatusHandlerFactory
{
    private readonly Dictionary<string?,IConfirmStatusHandler> _handlerMap;

    public MeasureCartableStatusHandlerFactory(Dictionary<string?,IConfirmStatusHandler> handlerMap)
    {
        _handlerMap = handlerMap;
    }

    IConfirmStatusHandler getHandler(string? entityName)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — entityName)
        throw new NotImplementedException();
    }
}
