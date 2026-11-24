using global::SemBackDotNet10.Base.Service.Inter;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
// Mechanical conversion from Java (service/interf/IEvaluationActionService.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.evaluationDomain.model.entity;
using SemBackDotNet10.evaluationDomain.model.dto;
using SemBackDotNet10.evaluationDomain.repository;
using SemBackDotNet10.evaluationDomain.service.interf;
using global::Common.model.entity;
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Dto;
using SemBackDotNet10.baseDomain.repository;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.actionDomain.model.entity;
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
using global::Common.logging;
using global::Common.model.dto;
using global::Common.utils.cartable;
using global::Common.utils.security;
using global::Common.utils.userRoles;


namespace SemBackDotNet10.evaluationDomain.service.interf;

/// <summary>معادل IEvaluationActionService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IEvaluationActionService : IProcessService<EvaluationAction,long?,global::SemBackDotNet10.evaluationDomain.repository.IEvaluationActionRepository>
{
    EvaluationAction findByIdOrElseThrow(long? id);
    SearchDto loadGridEvaluationAction(string? body, int page, int size, string? orderBy, SortDirection direction);
    void insertDataToEvaluationAction(string? body);
    SearchDto findAllEvaluationActionDetails(string? body, int page, int size, string? orderBy, SortDirection direction);
    EvaluationActionDto updateEvaluationAction(EvaluationActionDto evaluationActionDto);
    void editConfirmStatus(EvaluationActionDto evaluationActionDto);
    EvaluationActionDto getEvaluationActionById(long? evaluationActionId);
    void findAndDeleteEvaluationAction(List<ActionActPlan> actionActPlans);
    void updateDisabledStatus(List<EvaluationActionDto> evaluationActionDtoList);
    void deleteEvaluationActionByCatalogId(long? actionPlanCatalogId);
}
