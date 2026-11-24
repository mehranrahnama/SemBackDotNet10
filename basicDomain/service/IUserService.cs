using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using System;

namespace SemBackDotNet10.basicDomain.service;

public interface IUserService
{
    SearchDto FindAll(string search, int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction, long levelId);
    UserDto MapToDto(User user);
    UserDto MapToDtoV2(User user);
    User Edit(UserDto userDto);
    User FindByIdOrElseThrow(long id);
    Tenant FindUserTenantByUserName(string userName);
    User FindByUserName(string userName);
    long CountActiveUsers(long planId);
    long CountActiveUsersByPlanIdLevelId(long planId, long levelId);
    UserDto GetUserDetails(string body);
    bool UserIsAdmin(long userId);
    bool UserIsRater(long userId);
    bool UserIsMasterRater(long userId);
    bool UserIsModir(long userId);
    UserDto CreateUser(UserDto userDto);
    UserDto CreateUserFromSSO(UserDto userDto);
    void Delete(long id);
    bool LoginDisabled(string userName);
    DateTime LoginDisabledTime(string userName);
    SearchDto GetAllTashilgar(int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction, string body);
    SearchDto GetAllModir(int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction, string body);
    void DeactivateUser(long userId);
    void UnlockUserByUserId(long userId);
    void SynchronizeWithActiveDirectory();
}
