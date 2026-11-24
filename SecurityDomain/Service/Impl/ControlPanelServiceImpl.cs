using global::SemBackDotNet10.SecurityDomain.Service.Interf;
using global::SemBackDotNet10.service.impl;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SemLogger = global::Common.logging.SemLogger;
using SemBackDotNet10.Common.exception;
using DateUtils = global::Common.utils.DateUtils;
using LocaleUtils = global::Common.utils.LocaleUtils;
using PageUtils = global::Common.utils.PageUtils;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;

namespace SemBackDotNet10.SecurityDomain.Service.Impl;

/// <summary>
/// معادل ControlPanelServiceImpl.java.
/// وابستگی‌های اعلام‌شده: IControlPanelRepository، IUserService، IPlanPeriodService،
///   IPlaningLevelService، IControlPanelDto، DateUtils (CurrentDateTimePersianInString، Compare)،
///   LocaleUtils.GetInstance، PageUtils، SecurityUtils.GetCurrentUser، SearchDto — همه هنوز تبدیل نشده‌اند.
/// SortDirection direction جاوا → رشته‌ی "ASC"/"DESC".
/// </summary>
public class ControlPanelServiceImpl : IControlPanelService
{
    private readonly IControlPanelRepository _controlPanelRepository;
    private readonly IUserService _userService;
    private readonly IPlanPeriodService _planPeriodService;
    private readonly PlaningLevelServiceImpl _planingLevelService;
    private readonly SemLogger _semLogger;

    private readonly LocaleUtils _localeUtils;

    public ControlPanelServiceImpl(IControlPanelRepository controlPanelRepository, IUserService userService,
        IPlanPeriodService planPeriodService, PlaningLevelServiceImpl planingLevelService, SemLogger semLogger,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _localeUtils = new LocaleUtils(configuration);
        _controlPanelRepository = controlPanelRepository;
        _userService = userService;
        _planPeriodService = planPeriodService;
        _planingLevelService = planingLevelService;
        _semLogger = semLogger;
    }

    public IControlPanelRepository GetRepository() => _controlPanelRepository;

    public ControlPanel Create(ControlPanelDto controlPanelDto)
    {
        _semLogger.Info("Enter the method");
        ArgumentNullException.ThrowIfNull(controlPanelDto, "ControlPanelDto is Null");

        var controlPanel = new ControlPanel
        {
            Regdatestart = controlPanelDto.Regdatestart,
            Regdateend = controlPanelDto.Regdateend,
            ValdateStart = controlPanelDto.Valdatestart,
            Valdateend = controlPanelDto.Valdateend,
            Val_month = controlPanelDto.Val_month,
            Yearvalspec = controlPanelDto.Yearvalspec,
            Monthvalspec = controlPanelDto.Monthvalspec,
            Entitytype = controlPanelDto.Entitytype,
            PlaningLevel = controlPanelDto.Levelidallowedspec == null
                ? null
                : _planingLevelService.FindByIdOrElseThrowAsync(controlPanelDto.Levelidallowedspec.Value).GetAwaiter().GetResult(),
            Tashilgar = controlPanelDto.Tashilgaridallowedspec == null
                ? null
                : _userService.FindByIdOrElseThrow(controlPanelDto.Tashilgaridallowedspec.Value),
            Alllevelsallowedspec = controlPanelDto.Alllevelsallowedspec,
            Alltashilgarsallowedspec = controlPanelDto.Alltashilgarsallowedspec,
            AllowedPlaningLevelIds = controlPanelDto.AllowedPlaningLevelIds
        };

        controlPanel = GetRepository().Save(controlPanel);
        _semLogger.Info("Exit method");
        return controlPanel;
    }

    public ControlPanel Edit(ControlPanelDto controlPanelDto)
    {
        _semLogger.Info("Enter the method");
        ArgumentNullException.ThrowIfNull(controlPanelDto, "controlPanelDto is Null");

        var controlPanel = FindByIdOrElseThrow(controlPanelDto.Id);
        controlPanel.Regdatestart = controlPanelDto.Regdatestart;
        controlPanel.Regdateend = controlPanelDto.Regdateend;
        controlPanel.ValdateStart = controlPanelDto.Valdatestart;
        controlPanel.Valdateend = controlPanelDto.Valdateend;
        controlPanel.Yearvalspec = controlPanelDto.Yearvalspec;
        controlPanel.Monthvalspec = controlPanelDto.Monthvalspec;
        controlPanel.Val_month = controlPanelDto.Val_month;
        controlPanel.Entitytype = controlPanelDto.Entitytype;
        controlPanel.Alllevelsallowedspec = controlPanelDto.Alllevelsallowedspec;
        controlPanel.AllowedPlaningLevelIds = controlPanelDto.AllowedPlaningLevelIds;
        controlPanel = GetRepository().Save(controlPanel);

        _semLogger.Info("Exit method");
        return controlPanel;
    }

    public ControlPanelDto MapToDto(ControlPanel controlPanel)
    {
        _semLogger.Info("Enter the method");
        ArgumentNullException.ThrowIfNull(controlPanel, "controlPanel is Null");

        controlPanel = FindByIdOrElseThrow(controlPanel.Id);
        var controlPanelDto = new ControlPanelDto
        {
            Entitytype = controlPanel.Entitytype,
            Regdatestart = controlPanel.Regdatestart,
            Regdateend = controlPanel.Regdateend,
            Valdatestart = controlPanel.ValdateStart,
            Valdateend = controlPanel.Valdateend,
            Yearvalspec = controlPanel.Yearvalspec,
            Monthvalspec = controlPanel.Monthvalspec,
            Alllevelsallowedspec = controlPanel.Alllevelsallowedspec,
            Levelidallowedspec = controlPanel.PlaningLevel == null ? null : controlPanel.PlaningLevel.Id,
            Alltashilgarsallowedspec = controlPanel.Alltashilgarsallowedspec,
            Tashilgaridallowedspec = controlPanel.Tashilgar?.UserId,
            PlaningLevelDes = controlPanel.PlaningLevel != null ? controlPanel.PlaningLevel.DesLevel : null,
            UserName = controlPanel.Tashilgar != null
                ? controlPanel.Tashilgar.NameF + " " + controlPanel.Tashilgar.FamilyF
                : null,
            AllowedPlaningLevelIds = controlPanel.AllowedPlaningLevelIds
        };
        _semLogger.Info("Exit method");
        return controlPanelDto;
    }

    public ControlPanel FindByIdOrElseThrow(long? id)
    {
        _semLogger.Info("Enter the method");
        if (id == null)
        {
            throw new BusinessException("error.ControlPanel.idIsNotValid");
        }
        var result = GetRepository().FindById(id.Value)
            ?? throw new BusinessException("error.ControlPanel.idIsNotValid");
        _semLogger.Info("Exit method");
        return result;
    }

    public SearchDto FindAllByEntityType(Dictionary<string, object?> paramMapList, int page, int size,
        string? orderBy, string? direction)
    {
        _semLogger.Info("Enter the method");
        global::SemBackDotNet10.Common.enums.SortDirection? sortDirection = direction is null
            ? null
            : Enum.Parse<global::SemBackDotNet10.Common.enums.SortDirection>(direction, ignoreCase: true);
        var pageable = PageUtils.GetInstance(page, size, orderBy, sortDirection);
        if (paramMapList.TryGetValue("entitytype", out var entityTypeValue) && entityTypeValue is not null)
        {
            int entitytype = Convert.ToInt32(entityTypeValue);

            paramMapList.TryGetValue("filter", out var filterValue);
            string? filter = (string?)filterValue;
            var controlPanelDto = _controlPanelRepository.FindAllControlPanelByEntitytype(entitytype, pageable, filter);

            _semLogger.Info("Exit method");
            return new SearchDto.Builder()
                .Content(controlPanelDto.Content)
                .TotalElements(controlPanelDto.TotalElements)
                .TotalPages(controlPanelDto.TotalPages)
                .CurrentPage(page)
                .PageSize(size)
                .Build();
        }
        _semLogger.Info("Exit method");
        return null!; // در جاوا هم null برمی‌گرداند (BusinessException ساخته ولی پرتاب نمی‌شد)
    }

    public Dictionary<string, string> GetCreateDates()
    {
        _semLogger.Info("Enter the method");
        var dates = new Dictionary<string, string>();
        var controlPanel = _controlPanelRepository.FindByEntityType(1);
        dates["startDate"] = controlPanel.Regdatestart;
        dates["endDate"] = controlPanel.Regdateend;
        _semLogger.Info("Exit method");
        return dates;
    }

    public Dictionary<string, string> GetInsertDates()
    {
        var dates = new Dictionary<string, string>();
        var controlPanel = _controlPanelRepository.FindByEntityType(2);
        dates["startDate"] = controlPanel.ValdateStart;
        dates["endDate"] = controlPanel.Valdateend;
        return dates;
    }

    public bool HasAccessForUpdatingProgressPercentage(long? month, long? levelId)
    {
        _semLogger.Info("Enter the method");
        if (levelId == null)
            return false;
        // used in measure/action update percentage of progress mode
        var hasAccess = false;
        var controlPanel = _controlPanelRepository.FindByEntityType(2);

        var currentDate = CurrentControlPanelDate();

        if (month == 0 && DateUtils.CompareDate(currentDate, controlPanel.ValdateStart) >= 0 &&
            DateUtils.CompareDate(currentDate, controlPanel.Valdateend) <= 0)
            hasAccess = true;

        if (DateUtils.CompareDate(currentDate, controlPanel.ValdateStart) >= 0 &&
            DateUtils.CompareDate(currentDate, controlPanel.Valdateend) <= 0)
        {
            if (controlPanel.Val_month != null &&
                controlPanel.Val_month.Split(';').Contains(month.ToString()))
            {
                hasAccess = true;
            }
            else
            {
                var currentUserId = SecurityUtils.GetCurrentUserId() ?? throw new UnAuthorizedException();
                var type3ControlPanels =
                    _controlPanelRepository.FindByEntityTypeAndTashilgarIdAndLevelId(3, currentUserId, levelId.Value);
                foreach (var cp in type3ControlPanels)
                {
                    if (cp.Monthvalspec != null && cp.Monthvalspec.Split(';').Contains(month.ToString()))
                    {
                        hasAccess = true;
                        return hasAccess;
                    }
                }
            }
        }
        else
        {
            hasAccess = false;
        }
        _semLogger.Info("Exit method");
        return hasAccess;
    }

    public bool HasAccessForDefineShakhesEghdam(long? levelId)
    {
        _semLogger.Info("Enter the method");
        if (levelId == null)
            return false;

        var hasAccess = false;
        var controlPanel = _controlPanelRepository.FindByEntityType(1);

        var currentDate = CurrentControlPanelDate();

        hasAccess = DateUtils.CompareDate(currentDate, controlPanel.Regdatestart) >= 0 &&
                    DateUtils.CompareDate(currentDate, controlPanel.Regdateend) <= 0;

        _semLogger.Info("Exit method");
        return hasAccess;
    }

    private string CurrentControlPanelDate() =>
        DateTime.Now.ToString("yyyy'/'MM'/'dd", _localeUtils.GetInstance());
}
