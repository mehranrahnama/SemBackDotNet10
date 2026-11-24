namespace SemBackDotNet10.basicDomain.Model.Dto;

public sealed record StrategyObjectPerspectiveDto(
    long? Id,
    string? Code,
    string? Title,
    double? Weight,
    long? PerspectiveId,
    string? PerspectiveTitle,
    long? ThemePerspectiveId,
    string? ThemeTitle);
