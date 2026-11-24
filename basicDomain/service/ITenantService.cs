using global::SemBackDotNet10.SecurityDomain.Model.Entity;
using System;

namespace SemBackDotNet10.basicDomain.service;

public interface ITenantService
{
    Tenant? FindById(long id);
    Tenant FindByIdOrElseThrow(long id);
    Tenant GetUserPrivateTenantObj(UserAccount account);
    void DeleteTenant(long id);
    Tenant FindByNameOrElseThrow(string defaultTenant);
    Tenant? GetDefaultUserTenant(string username);
    TenantDto MapToDto(Tenant tenant);
    Tenant? FindByIdAndUserName(string currentTenant, string username);
    Tenant FindByCodeOrElseThrow(string defaultTenant);
}
