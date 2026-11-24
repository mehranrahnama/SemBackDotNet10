// Mechanical conversion from Java (repository/ActionPlanCatalogLogRepository.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.baseDomain.repository;



namespace SemBackDotNet10.actionDomain.repository;

/// <summary>معادل ActionPlanCatalogLogRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface ActionPlanCatalogLogRepository : global::SemBackDotNet10.Base.repository.ISemRepository<ActionPlanCatalogLog,long?>
{
}
