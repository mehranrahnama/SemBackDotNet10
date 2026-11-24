// Mechanical conversion from Java (service/impl/CorrectionActionsByEMSServiceImpl.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Nodes;
using Common.config.properties;
using Common.webServices;
using SemBackDotNet10.Common.exception;
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


namespace SemBackDotNet10.actionDomain.service.impl;

/// <summary>معادل CorrectionActionsByEMSServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class CorrectionActionsByEMSServiceImpl : ICorrectionActionsByEMSService
{
    private readonly CorrectionActionsByEMSRepository _correctionActionsByEMSRepository;
    private readonly CustomRestTemplateService _customRestTemplateService;
    private readonly CorrectionActionsProperties _correctionActionsProperties;
    private readonly ICorrectionActionByEMSMapper _correctionActionByEMSMapper;
    private const string _CORRECTION_ACTIONS_TITLE_FIELD = "title";
    private const string _CORRECTION_ACTIONS_COMMITTEE_ID_FIELD = "chartId";
    private const string _CORRECTION_ACTIONS_COMMITTEE_NAME_FIELD = "chartTitle";
    private const string _CORRECTION_ACTIONS_IDENTITY_CODE_FIELD = "identityCode";
    private const string _CORRECTION_ACTIONS_STATUS_FIELD = "status";
    private const string _DEFAULT_SYSTEM_STANDARD_CODE_VALUE = "SEM";
    private const string _CORRECTION_ACTIONS_EMS_ID = "id";
    private readonly IHostEnvironment _environment;

    public CorrectionActionsByEMSServiceImpl(CorrectionActionsByEMSRepository correctionActionsByEMSRepository, CustomRestTemplateService customRestTemplateService, CorrectionActionsProperties correctionActionsProperties, ICorrectionActionByEMSMapper correctionActionByEMSMapper, IHostEnvironment environment)
    {
        _correctionActionsByEMSRepository = correctionActionsByEMSRepository;
        _customRestTemplateService = customRestTemplateService;
        _correctionActionsProperties = correctionActionsProperties;
        _correctionActionByEMSMapper = correctionActionByEMSMapper;
        _environment = environment;
    }

    public void saveCorrectionActionsByEMS(Dictionary<string?,string?> headers, string? systemStandardCode, string? actionOriginCode, string? identityCode, string? status, string? showInactives, string? date)
    {
        try
        {
            var url = extractedUrl(systemStandardCode, actionOriginCode, identityCode, status, showInactives, date);
            var response = RequestCorrectionActions(url!, headers);
            if (response is not JsonArray array || array.Count == 0) return;
            _correctionActionsByEMSRepository.SaveAllAsync(prepareCorrectionAtionList(array)).GetAwaiter().GetResult();
        }
        catch (Exception)
        {
            throw new BusinessException("عدم دسترسی به سامانه ثبت اقدام اصلاحی");
        }
    }
    public CorrectionActionByEMSDto retrieveCorrectionActionByEMSWithIdentityCode(Dictionary<string?,string?> headers, string? identityCode)
    {
        try
        {
            var response = RequestCorrectionActions(extractedUrl(identityCode)!, headers);
            if (response is JsonArray array && array.Count != 0)
                return _correctionActionByEMSMapper.toDto(prepareCorrectionAtionList(array)[0]);
            throw new BusinessException("شناسه نامعتبر");
        }
        catch (BusinessException) { throw; }
        catch (Exception)
        {
            throw new BusinessException("عدم دسترسی به سامانه ثبت اقدام اصلاحی");
        }
    }
    public List<CorrectionActionByEMSDto>? retrieveAllCorrectionActionsByEMS(Dictionary<string?,string?> headers)
    {
        try
        {
            var response = RequestCorrectionActions(extractedUrl()!, headers);
            if (response is not JsonArray array || array.Count == 0) return null;
            return prepareCorrectionAtionList(array).Select(_correctionActionByEMSMapper.toDto).ToList();
        }
        catch (Exception)
        {
            if (_environment.IsDevelopment() || _environment.IsEnvironment("dev")) return null;
            throw new BusinessException("عدم دسترسی به سامانه ثبت اقدام اصلاحی");
        }
    }
    List<CorrectionActionsByEMS> prepareCorrectionAtionList(JsonNode correctionActionsResponse)
    {
        var array = correctionActionsResponse as JsonArray ?? throw new FormatException("EMS response must be an array.");
        string Text(JsonNode node, string? key) => node[key ?? throw new InvalidOperationException("EMS field name is missing.")]?.ToString()
            ?? throw new FormatException("EMS response field is missing.");
        long Number(JsonNode node, string? key) => long.TryParse(Text(node, key), out var value) ? value : 0L;
        return array.Select(item =>
        {
            var node = item ?? throw new FormatException("EMS response item is null.");
            return new CorrectionActionsByEMS
            {
                Title = Text(node, _CORRECTION_ACTIONS_TITLE_FIELD),
                CommitteeId = Number(node, _CORRECTION_ACTIONS_COMMITTEE_ID_FIELD),
                CommitteeName = Text(node, _CORRECTION_ACTIONS_COMMITTEE_NAME_FIELD),
                IdentityCode = Number(node, _CORRECTION_ACTIONS_IDENTITY_CODE_FIELD),
                EmsId = Number(node, _CORRECTION_ACTIONS_EMS_ID),
                Status = Text(node, _CORRECTION_ACTIONS_STATUS_FIELD)
            };
        }).ToList();
    }
    string? extractedUrl(string? identityCode)
    {
        var parameter = _correctionActionsProperties.QueryParams?.IdentityCode
            ?? throw new InvalidOperationException("EMS identity parameter is not configured.");
        return QueryHelpers.AddQueryString(extractedUrl()!, parameter, identityCode ?? string.Empty);
    }
    string? extractedUrl()
    {
        var url = _correctionActionsProperties.Url ?? throw new InvalidOperationException("EMS URL is not configured.");
        var parameter = _correctionActionsProperties.QueryParams?.SystemStandardCode
            ?? throw new InvalidOperationException("EMS system-standard parameter is not configured.");
        return QueryHelpers.AddQueryString(url, parameter, _DEFAULT_SYSTEM_STANDARD_CODE_VALUE ?? string.Empty);
    }
    string? extractedUrl(string? systemStandardCode, string? actionOriginCode, string? identityCode, string? status, string? showInactives, string? date)
    {
        var url = _correctionActionsProperties.Url ?? throw new InvalidOperationException("EMS URL is not configured.");
        var names = _correctionActionsProperties.QueryParams ?? throw new InvalidOperationException("EMS query parameters are not configured.");
        void Add(string? name, string? value)
        {
            if (string.IsNullOrEmpty(value)) return;
            url = QueryHelpers.AddQueryString(url, name ?? throw new InvalidOperationException("EMS parameter name is missing."), value);
        }
        Add(names.SystemStandardCode, systemStandardCode);
        Add(names.ActionOriginCode, actionOriginCode);
        Add(names.IdentityCode, identityCode);
        Add(names.Status, status);
        Add(names.ShowInactives, showInactives);
        Add(names.Date, date);
        return url;
    }

    private JsonNode? RequestCorrectionActions(string url, Dictionary<string?, string?> headers)
    {
        var body = headers.Where(x => x.Key != null).ToDictionary(x => x.Key!, x => x.Value!);
        return _customRestTemplateService.SignInRestApi(url, body).GetAwaiter().GetResult();
    }
}
