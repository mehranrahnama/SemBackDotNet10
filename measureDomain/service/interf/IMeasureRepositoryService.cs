using MeasureRepository = global::SemBackDotNet10.measureDomain.model.entity.MeasureRepository;
using global::SemBackDotNet10.Base.Service.Inter;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
// Mechanical conversion from Java (service/interf/IMeasureRepositoryService.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.measureDomain.model.entity;
using SemBackDotNet10.measureDomain.model.dto;
using SemBackDotNet10.measureDomain.controller.dto;
using SemBackDotNet10.measureDomain.mapper;
using SemBackDotNet10.measureDomain.repository;
using SemBackDotNet10.measureDomain.service.interf;
using SemBackDotNet10.measureDomain.domain;
using SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.context;
using SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.factory;
using SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.handler;
using global::Common.model.entity;
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.baseDomain.repository;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;
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


namespace SemBackDotNet10.measureDomain.service.interf;

/// <summary>معادل IMeasureRepositoryService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IMeasureRepositoryService : IProcessService<MeasureRepository,long?,MeasureRepoRepository>
{
    MeasureRepoRepository getRepository();
    MeasureRepository findByIdOrElseThrow(long? id);
    MeasureRepository findMeasurePeriod(long? id);
    SearchDto findAll(string? Search, int page, int size, string? orderBy, SortDirection direction);
    MeasureUnitDto getMeasureUnit(long? repositoryId);
    void delete(long? id);
    MeasureRepository create_V2(MeasureRepositoryDto measureRepositoryDto, global::Microsoft.AspNetCore.Http.HttpRequest request);
    MeasureRepository edit(MeasureRepositoryDto measureRepositoryDto);
    MeasureRepositoryDto mapToDto(MeasureRepository measureRepository);
    MeasureRepository mapToEntity(MeasureRepositoryDto measureRepositoryDto);
    MeasureRepositoryDto getMeasureRepositoryDto(long? id);
    MeasureRepositoryDto findMeasureRepositoryById(long? MeasureRepositoryId);
    MeasureRepositoryDto findMeasureRepositoryByMeasureCatalogId(long? MeasureCatalogId);
    string? getAllMeasureRepositories_reportDataSet(string? filter);
    bool hasMeasureActPlan(long? measureRepositoryId);
    MeasureRepository findOrInsertProcessMeasureRepository(System.Text.Json.Nodes.JsonObject measureProcess, MeasureUnit measureUnit);
    void updateDesiredMethodAndPayeshType(int desiredMethod, byte measurePayeshType, long? measureId);
    void restoreMeasureRepositoryFromLog(int desiredMethod, byte measurePayeshType, long? measureId);
    bool hasDesiredMethodOrPayeshTypeChanged(long? measureId, byte measurePayeshType, int desiredMethod);
}
