using System.Collections.Generic;
using SemBackDotNet10.service.impl;
using SemBackDotNet10.basicDomain.Model.Dto;

namespace SemBackDotNet10.basicDomain.service;

public interface ISettingService
{
    Setting CreateSetting(long tenantId, string key, string value);
    List<SettingDTO> GetAllByTenantId(long tenantId);
    Setting EditSetting(Setting setting);
    bool RemoveSetting(long settingId);
    string GetSettingByKeyAndTenantId(long tenantId, string expireDate);
    string GetTenantName();
}
