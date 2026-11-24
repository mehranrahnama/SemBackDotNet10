using global::SemBackDotNet10.Base.Service.Inter;
// Mechanical conversion from Java (service/interf/IEvaluationFeedBackService.java). Complex bodies are marked with TODO.
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

/// <summary>معادل IEvaluationFeedBackService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IEvaluationFeedBackService : IBaseService<EvaluationFeedBack,long?,global::SemBackDotNet10.evaluationDomain.repository.IEvaluationFeedBackRepository>
{
    void create(EvaluationFeedBackDto evaluationFeedBackDto);
    List<EvaluationFeedBackDto> getAllByLevelIdAndEvaluationPeriod(EvaluationFeedBackDto evaluationFeedBackDto);
    void edit(EvaluationFeedBackDto evaluationFeedBackDto);
    void delete(long? evaluationFeedBackId);
    EvaluationFeedBack findByIdOrElseThrow(long? id);
    EvaluationFeedBackDto findEvaluationFeedbackById(long? id);
    void editConfirmStatus(string? outCome, long? id);
}
