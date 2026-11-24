using Common.exception;
using SemBackDotNet10.riskDomain.model.entity;
using SemBackDotNet10.riskDomain.repository;

namespace SemBackDotNet10.riskDomain.domain;

public interface IRiskDomainService
{
    Task<Risk> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);
}

public class RiskDomainService : IRiskDomainService
{
    private readonly IRiskRepository _riskRepository;

    public RiskDomainService(IRiskRepository riskRepository)
    {
        _riskRepository = riskRepository;
    }

    public async Task<Risk> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.Risk.idIsNull");
        return await _riskRepository.FindByIdAsync(id.Value, ct)
               ?? throw new BusinessException("", id.Value.ToString());
    }
}
