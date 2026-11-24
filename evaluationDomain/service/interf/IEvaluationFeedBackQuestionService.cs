using global::SemBackDotNet10.Base.Service.Inter;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
// Mechanical conversion from Java (service/interf/IEvaluationFeedBackQuestionService.java). Complex bodies are marked with TODO.
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

/// <summary>معادل IEvaluationFeedBackQuestionService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IEvaluationFeedBackQuestionService : IBaseService<EvaluationFeedBackQuestion,long?,global::SemBackDotNet10.evaluationDomain.repository.IEvaluationFeedBackQuestionRepository>
{
    EvaluationFeedBackQuestion findByIdOrElseThrow(long? id);
    SearchDto getAllEvaluationFeedBackQuestion(int page, int size, string? orderBy, SortDirection direction, string? body);
    void createEvaluationFeedBackQuestion(EvaluationFeedBackQuestionDto evaluationFeedBackQuestionDto);
    void editEvaluationFeedBackQuestion(EvaluationFeedBackQuestionDto evaluationFeedBackQuestionDto);
    void deleteEvaluationFeedBackQuestion(EvaluationFeedBackQuestionDto evaluationFeedBackQuestionDto);
    void saveQuestionWeights(List<EvaluationFeedBackQuestionDto> dto);
}
