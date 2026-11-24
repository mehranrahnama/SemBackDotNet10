using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
// Mechanical conversion from Java (service/impl/EvaluationTeamHeaderServiceImpl.java). Complex bodies are marked with TODO.
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


namespace SemBackDotNet10.evaluationDomain.service.impl;

/// <summary>معادل EvaluationTeamHeaderServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class EvaluationTeamHeaderServiceImpl(global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamHeaderRepository evaluationTeamHeaderRepository) : IEvaluationTeamHeaderService
{
    public global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamHeaderRepository GetRepository() => evaluationTeamHeaderRepository;
    public global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamHeaderRepository getRepository() => GetRepository();
    public EvaluationTeamHeaderDto create(EvaluationTeamHeaderDto evaluationTeamHeaderDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationTeamHeaderDto)
        throw new NotImplementedException();
    }
    EvaluationTeamHeaderDto mapToDto(EvaluationTeamHeader evaluationTeamHeader)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationTeamHeader)
        throw new NotImplementedException();
    }
    public SearchDto findAllMasterRater(int page, int size, string? orderBy, SortDirection direction, string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    public EvaluationTeamHeader findByIdOrElseThrow(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    public void deleteMasterRater(long? evaluationTeamHeaderId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationTeamHeaderId)
        throw new NotImplementedException();
    }
    public void editMasterRater(EvaluationTeamHeaderDto evaluationTeamHeaderDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationTeamHeaderDto)
        throw new NotImplementedException();
    }
    public List<EvaluationTeamHeaderLevelDto> getEvaluationPlaningLevel(EvaluationTeamHeaderDto evaluationTeamHeaderDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationTeamHeaderDto)
        throw new NotImplementedException();
    }
    bool? checkUnAllowedEdit(EvaluationTeamHeaderDto evaluationTeamHeaderDto, EvaluationTeamHeader evaluationTeamHeader)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationTeamHeader)
        throw new NotImplementedException();
    }
}
