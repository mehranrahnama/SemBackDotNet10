using global::SemBackDotNet10.Common.Pagination;
// Mechanical conversion from Java (service/impl/EvaluationTeamServiceImpl.java). Complex bodies are marked with TODO.
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

/// <summary>معادل EvaluationTeamServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class EvaluationTeamServiceImpl(global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamRepository evaluationTeamRepository) : IEvaluationTeamService
{
    public global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamRepository GetRepository() => evaluationTeamRepository;
    public global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamRepository getRepository() => GetRepository();
    public EvaluationTeam findByIdOrElseThrow(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    public List<EvaluationTeamDto> create(List<EvaluationTeamDto> evaluationTeamDtos)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationTeamDtos)
        throw new NotImplementedException();
    }
    public PagedResult<EvaluationTeamDto> findAllRater(int page, int size, string? orderBy, SortDirection direction, long? evaluationTeamHeaderLevelId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationTeamHeaderLevelId)
        throw new NotImplementedException();
    }
    public void deleteRater(long? evaluationTeamId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationTeamId)
        throw new NotImplementedException();
    }
    public void editRater(EvaluationTeamDto evaluationTeamDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationTeamDto)
        throw new NotImplementedException();
    }
}
