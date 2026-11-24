using Common.model.dto;
using Common.model.entity;
namespace Common.repository;

// Read-side replacement only; inherited external SemRepository CRUD is not supplied.
public interface IDesRepository
{
    Des? FindById(long id);
    List<DesDto> GetAllActionTypes();
    List<DesDto> GetAllActionPriorities();
    List<DesDto> GetAllActionMeasureRepositoryStatus();
    List<DesDto> GetAllCertificate();
}

// The host supplies an IQueryable from its database provider and owns its lifetime.
// SQL translation and database collation must be integration-tested by the host.
public sealed class DesRepository(IQueryable<Des> source) : IDesRepository
{
    private readonly IQueryable<Des> source = source ?? throw new ArgumentNullException(nameof(source));
    public Des? FindById(long id) => source.SingleOrDefault(x => x.Id == id);
    private List<DesDto> Group(long id) => source.Where(x => x.DesGrp != null && x.DesGrp.Id == id)
        .Select(x => new DesDto { Val = x.Val, DesF = x.DesF }).ToList();
    public List<DesDto> GetAllActionTypes() => Group(26);
    public List<DesDto> GetAllActionPriorities() => Group(30);
    public List<DesDto> GetAllActionMeasureRepositoryStatus() => Group(31);
    public List<DesDto> GetAllCertificate() => source.Where(x => x.DesGrp != null && x.DesGrp.DesGrpE == "Certificate")
        .Select(x => new DesDto { Id = x.Id, DesF = x.DesF }).ToList();
}
