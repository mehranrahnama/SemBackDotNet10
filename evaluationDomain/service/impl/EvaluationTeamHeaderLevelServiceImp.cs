using EvalLevel = SemBackDotNet10.evaluationDomain.model.entity.EvaluationTeamHeaderLevel;
// Mechanical conversion from Java (service/impl/EvaluationTeamHeaderLevelServiceImp.java). Complex bodies are marked with TODO.
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

/// <summary>معادل EvaluationTeamHeaderLevelServiceImp.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class EvaluationTeamHeaderLevelServiceImp(global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamHeaderLevelRepository evaluationTeamHeaderLevelRepository, SemDbContext db, SemBackDotNet10.service.impl.PlaningLevelUserServiceImpl memberships) : IEvaluationTeamHeaderLevelService
{
    public global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamHeaderLevelRepository GetRepository() => evaluationTeamHeaderLevelRepository;
    public global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamHeaderLevelRepository getRepository() => GetRepository();
    public EvalLevel findByIdOrElseThrow(long? id)
    {
        if (id is null) throw new BusinessException("error.EvaluationTeamHeaderLevel.idIsNotValid");
        return evaluationTeamHeaderLevelRepository.FindByIdAsync(id).GetAwaiter().GetResult()
            ?? throw new BusinessException("error.EvaluationTeamHeaderLevel.idIsNotValid");
    }
    public EvaluationTeamHeaderLevelDto mapToDto(EvalLevel evaluationTeamHeaderLevel)
    {
        ArgumentNullException.ThrowIfNull(evaluationTeamHeaderLevel);
        return new EvaluationTeamHeaderLevelDto { Id = evaluationTeamHeaderLevel.Id,
            LevelId = evaluationTeamHeaderLevel.PlaningLevel.Id,
            DesLevel = evaluationTeamHeaderLevel.PlaningLevel.DesLevel };
    }
    public void create(List<long?> levelIds, EvaluationTeamHeader evaluationTeamHeader)
    {
        ArgumentNullException.ThrowIfNull(levelIds);
        ArgumentNullException.ThrowIfNull(evaluationTeamHeader);
        if (evaluationTeamHeader.Id is null) throw new ArgumentException("Evaluation team header must already be saved.", nameof(evaluationTeamHeader));
        var header = db.Set<EvaluationTeamHeader>().Find(evaluationTeamHeader.Id)
            ?? throw new BusinessException("error.EvaluationTeamHeader.idIsNotValid");
        var items = new List<EvalLevel>();
        foreach (var levelId in levelIds)
        {
            var level = levelId is null ? null : db.PlaningLevels.Find(levelId.Value)
                ?? throw new BusinessException("error.PlaningLevel.idIsNotValid");
            items.Add(new EvalLevel { EvaluationTeamHeader = header, PlaningLevel = level! });
        }
        evaluationTeamHeaderLevelRepository.SaveAllAsync(items).GetAwaiter().GetResult();
    }
    public List<long?> getUserLevelIds(long? userId)
    {
        if (userId is null) throw new ArgumentNullException(nameof(userId));
        return memberships.GetUserLevelIdsAsync(userId.Value).GetAwaiter().GetResult().Select(x => (long?)x)
            .Union(evaluationTeamHeaderLevelRepository.getCurrentUserLevelIds(userId)).ToList();
    }
    public List<long?> getUserLevelId(long? userId, long? planId)
    {
        return evaluationTeamHeaderLevelRepository.findLevelIdsByUserIdAndPlanId(userId, planId);
    }
}
