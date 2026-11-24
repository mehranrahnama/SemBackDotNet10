using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using System.Collections.Generic;

namespace SemBackDotNet10.basicDomain.service;

public interface IStrategyPillarsMeasureService
{
    void CreateNewStrategyPillarsMeasure(List<StrategyPillarsMeasureDto> strategyPillarsMeasureDto);
    SearchDto GetAllStrategyPillarsMeasuresByStrategyPillarsId(int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction, long strategyPillarsId, string filter);
    StrategyPillarsMeasureDto GetAllStrategyPillarsMeasuresById(long id);
    void EditStrategyPillarsMeasureById(List<StrategyPillarsMeasureDto> strategyPillarsMeasureDto);
    void SoftDeleteStrategyPillarsMeasureById(string id);
}
