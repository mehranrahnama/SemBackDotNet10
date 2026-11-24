using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
namespace SemBackDotNet10.basicDomain.service;

public interface IPlaningLevelTeamService
{
    SearchDto FindAll(string search, int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction);
    SearchDto FindAllByLevelId(string filter, int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction);
    PlaningLevelTeamDto MapToDto(PlaningLevelTeam planingLevelTeam);
    PlaningLevelTeam MapToEntity(PlaningLevelTeamDto planingLevelTeamDto);
    PlaningLevelTeam Edit(PlaningLevelTeamDto planingLevelTeamDto);
    PlaningLevelTeam FindByIdOrElseThrow(long id);
    PlaningLevelTeam Create(PlaningLevelTeamDto planingLevelTeamDto);
    void Delete(long id);
}
