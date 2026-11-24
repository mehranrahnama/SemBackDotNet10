using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
namespace SemBackDotNet10.basicDomain.service;

public interface IPositionService
{
    SearchDto FindAll(string search, int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction);
    PositionDto MapToDto(Position position);
    SearchDto FindPositions(string filter, int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction);
    Position FindByIdOrElseThrow(long id);
    void HardDeletePositionById(long id);
    PositionDto EditPositionById(PositionDto positionDto);
    PositionDto CreateNewPosition(PositionDto positionDto);
}
