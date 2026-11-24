// Mechanical conversion from Java (mapper/ICorrectionActionByEMSMapper.java). Complex bodies are marked with TODO.
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




namespace SemBackDotNet10.actionDomain.mapper;

/// <summary>معادل ICorrectionActionByEMSMapper.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface ICorrectionActionByEMSMapper
{
    // @Mapping(source = "entity.title", target = "title")
    // @Mapping(source = "entity.committeeId", target = "committeeId")
    // @Mapping(source = "entity.committeeName", target = "committeeName")
    // @Mapping(source = "entity.status", target = "status")
    // @Mapping(source = "entity.identityCode", target = "identityCode")
    // @Mapping(source = "entity.emsId", target = "id")
    CorrectionActionByEMSDto toDto(CorrectionActionsByEMS entity);
}
