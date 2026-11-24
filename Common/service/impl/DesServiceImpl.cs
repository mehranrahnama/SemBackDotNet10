using Common.model.dto;
using Common.model.entity;
using Common.repository;
using Common.service.interf;
using Common.exception;
using Microsoft.Extensions.Logging;
namespace Common.service.impl;
public sealed class DesServiceImpl(IDesRepository repository, ILogger<DesServiceImpl> logger) : IDesService
{
    private readonly IDesRepository repository = repository ?? throw new ArgumentNullException(nameof(repository));
    public IDesRepository GetRepository() => repository;
    public List<DesDto> GetAllActionTypes() => repository.GetAllActionTypes();
    public List<DesDto> GetAllActionPriorities() => repository.GetAllActionPriorities();
    public List<DesDto> GetAllActionMeasureRepositoryStatus() => repository.GetAllActionMeasureRepositoryStatus();
    public List<DesDto> GetAllCertificate() => repository.GetAllCertificate();
    public Des FindByIdOrElseThrow(long? id)
    {
        logger.LogInformation("Enter the method");
        if (id is null) throw new BusinessException("error.Des.idIsNotValid");
        var result = repository.FindById(id.Value) ?? throw new BusinessException("error.Des.idIsNotValid", new[]{id.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)});
        logger.LogInformation("Exit method");
        return result;
    }
}
