
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;

namespace SemBackDotNet10.SecurityDomain.Service.Interf;

/// <summary>معادل ControlPanelService.java — اینترفیس سرویس پنل کنترل.</summary>
public interface IControlPanelService
{
    /// <summary>معادل متد BaseService: getRepository().</summary>
    IControlPanelRepository GetRepository();

    /// <summary>معادل create(ControlPanelDto).</summary>
    ControlPanel Create(ControlPanelDto controlPanelDto);

    /// <summary>throws Exception جاوا → در C# exceptionهای runtime منتقل می‌شوند.</summary>
    ControlPanel Edit(ControlPanelDto controlPanelDto);

    /// <summary>معادل mapToDto(ControlPanel).</summary>
    ControlPanelDto MapToDto(ControlPanel controlPanel);

    /// <summary>معادل findByIdOrElseThrow(Long).</summary>
    ControlPanel FindByIdOrElseThrow(long? id);

    /// <summary>معادل findAllByEntityType(Map, int, int, String, SortDirection direction).</summary>
    SearchDto FindAllByEntityType(Dictionary<string, object?> paramMapList, int page, int size, string? orderBy, string? direction);

    /// <summary>معادل getCreateDates().</summary>
    Dictionary<string, string> GetCreateDates();

    /// <summary>معادل getInsertDates().</summary>
    Dictionary<string, string> GetInsertDates();

    /// <summary>معادل hasAccessForUpdatingProgressPercentage(Long, Long).</summary>
    bool HasAccessForUpdatingProgressPercentage(long? month, long? levelId);

    /// <summary>معادل hasAccessForDefineShakhesEghdam(Long).</summary>
    bool HasAccessForDefineShakhesEghdam(long? levelId);
}
