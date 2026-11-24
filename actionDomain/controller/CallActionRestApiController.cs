// Mechanical conversion from Java (controller/CallActionRestApiController.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.controller.dto;
using SemBackDotNet10.actionDomain.mapper;
using SemBackDotNet10.actionDomain.repository;
using SemBackDotNet10.actionDomain.service.interf;
using SemBackDotNet10.actionDomain.domain;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.context;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.factory;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.handler;



namespace SemBackDotNet10.actionDomain.controller;

/// <summary>معادل CallActionRestApiController.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class CallActionRestApiController
{
    private readonly ICallActionRestApiService _callActionRestApiService;

    public CallActionRestApiController(ICallActionRestApiService callActionRestApiService)
    {
        _callActionRestApiService = callActionRestApiService;
    }

    [HttpPost("signIn")]
    IActionResult signIn(Dictionary<string?,string?> body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    [HttpPost("startActionProcess")]
    IActionResult startActionProcess(StartActionDTO body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
}
