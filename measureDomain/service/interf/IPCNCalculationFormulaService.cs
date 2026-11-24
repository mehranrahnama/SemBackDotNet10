using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
// Mechanical conversion from Java (service/interf/IPCNCalculationFormulaService.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.Base.Service.Inter;



namespace SemBackDotNet10.measureDomain.service.interf;

/// <summary>معادل IPCNCalculationFormulaService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IPCNCalculationFormulaService : IBaseService<PCNCalculationFormula,long?,PCNCalculationFormulaRepository>
{
    PCNCalculationFormula create(PCNCalculationFormulaDto pCNCalculationFormulaDto);
    PCNCalculationFormulaDto mapToDto(PCNCalculationFormula pCNCalculationFormula);
    PCNCalculationFormula mapToEntity(PCNCalculationFormulaDto pCNCalculationFormulaDto);
    SearchDto findAll(string? Search, int page, int size, string? orderBy, SortDirection direction);
    MeasureActPlan calculatePCNAchievedValue(MeasureActPlan measureActPlan);
    void calculatePCNForAllMeasureActPlans(string? filter);
}
