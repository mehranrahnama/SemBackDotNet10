namespace Common.repository;
// Domain entities are outside common: bind to real ActionPlanCatalog/PlaningLevel types in host.
public interface IPlaningLevelQuery<TPlaningLevel> where TPlaningLevel:class
{
    List<TPlaningLevel> FindAllPlaningLevels();
}
public sealed class ReportRepository<TActionPlanCatalog,TPlaningLevel>(IPersistenceSession<TActionPlanCatalog> session,System.Linq.Expressions.Expression<Func<TActionPlanCatalog,long?>> key,IPlaningLevelQuery<TPlaningLevel> levels):EntityRepository<TActionPlanCatalog,long?>(session,key)
    where TActionPlanCatalog:class where TPlaningLevel:class
{
    public List<TPlaningLevel> FindAllPlaningLevels()=>levels.FindAllPlaningLevels();
}
