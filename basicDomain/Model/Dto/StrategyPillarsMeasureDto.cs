#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemBackDotNet10.basicDomain.Model.Dto;

public class StrategyPillarsMeasureDto
{
    public long? Id { get; set; }
    [Required(ErrorMessage = "شناسه ارکان استراتژی نباید خالی باشد")]
    public long? StrategyPillarsId { get; set; }
    [Required(ErrorMessage = "نام شاخص نباید خالی باشد")]
    public string? MeasureName { get; set; }
    public string? MeasurePlan { get; set; }
    [Required(ErrorMessage = "شناسه واحد اندازه گیری نباید خالی باشد")]
    public long? MeasureUnitId { get; set; }
    [Required(ErrorMessage = "لطفا سال مرتبط با شاخص را مشخص کنید")]
    public long? Year { get; set; }
    public bool IsDeleted { get; set; }
}
