using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;

namespace SemBackDotNet10.analysisDomain.service.interf.porter;

public interface IPorterResultService
{
    Task<PorterResult> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task CreateNewPorterResultAsync(PorterResultDto porterResultDto, CancellationToken ct = default);

    Task<PorterResultDto?> GetAllByProductMarketAsync(string? body, CancellationToken ct = default);

    Task<PorterResultDto> EditPorterResultAsync(PorterResultDto porterResultDto, CancellationToken ct = default);
}
