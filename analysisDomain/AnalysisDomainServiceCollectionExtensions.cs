using SemBackDotNet10.analysisDomain.repository.pestel;
using SemBackDotNet10.analysisDomain.repository.porter;
using SemBackDotNet10.analysisDomain.repository.resource;
using SemBackDotNet10.analysisDomain.repository.stakeHolder;
using SemBackDotNet10.analysisDomain.service.impl.pestel;
using SemBackDotNet10.analysisDomain.service.impl.porter;
using SemBackDotNet10.analysisDomain.service.impl.resource;
using SemBackDotNet10.analysisDomain.service.impl.stakeHolder;
using SemBackDotNet10.analysisDomain.service.interf.pestel;
using SemBackDotNet10.analysisDomain.service.interf.porter;
using SemBackDotNet10.analysisDomain.service.interf.resource;
using SemBackDotNet10.analysisDomain.service.interf.stakeHolder;

namespace SemBackDotNet10.analysisDomain;

public static class AnalysisDomainServiceCollectionExtensions
{
    public static IServiceCollection AddAnalysisDomain(this IServiceCollection s)
    {
        s.AddScoped<IPestelRepository, PestelRepository>();
        s.AddScoped<IPestelGroupRepository, PestelGroupRepository>();
        s.AddScoped<IPestelScoreRepository, PestelScoreRepository>();
        s.AddScoped<IPestelService, PestelServiceImpl>();
        s.AddScoped<IPestelGroupService, PestelGroupServiceImpl>();
        s.AddScoped<IPestelScoreService, PestelScoreServiceImpl>();

        s.AddScoped<IAltProductRepository, AltProductRepository>();
        s.AddScoped<IMarketRepository, MarketRepository>();
        s.AddScoped<IPorterGroupRepository, PorterGroupRepository>();
        s.AddScoped<IPorterRepository, PorterRepository>();
        s.AddScoped<IPorterResultRepository, PorterResultRepository>();
        s.AddScoped<IPorterScoreRepository, PorterScoreRepository>();
        s.AddScoped<IProductGroupRepository, ProductGroupRepository>();
        s.AddScoped<IProductRepository, ProductRepository>();
        s.AddScoped<IProductThreatRepository, ProductThreatRepository>();
        s.AddScoped<IAltProductService, AltProductServiceImpl>();
        s.AddScoped<IMarketService, MarketServiceImpl>();
        s.AddScoped<IPorterGroupService, PorterGroupServiceImpl>();
        s.AddScoped<IPorterService, PorterServiceImpl>();
        s.AddScoped<IPorterResultService, PorterResultServiceImpl>();
        s.AddScoped<IPorterScoreService, PorterScoreServiceImpl>();
        s.AddScoped<IProductGroupService, ProductGroupServiceImpl>();
        s.AddScoped<IProductService, ProductServiceImpl>();
        s.AddScoped<IProductThreatService, ProductThreatServiceImpl>();

        s.AddScoped<IResourceRepository, ResourceRepository>();
        s.AddScoped<IResourceCompetencyRuleRepository, ResourceCompetencyRuleRepository>();
        s.AddScoped<IStakeHolderRepository, StakeHolderRepository>();
        s.AddScoped<IStakeHolderGroupRepository, StakeHolderGroupRepository>();
        s.AddScoped<IStakeHolderNeedsPriorityRepository, StakeHolderNeedsPriorityRepository>();
        s.AddScoped<IResourceService, ResourceServiceImpl>();
        s.AddScoped<IResourceCompetencyRuleService, ResourceCompetencyRuleServiceImpl>();
        s.AddScoped<IStakeHolderService, StakeHolderServiceImpl>();
        s.AddScoped<IStakeHolderGroupService, StakeHolderGroupServiceImpl>();
        s.AddScoped<IStakeHolderNeedsPriorityService, StakeHolderNeedsPriorityServiceImpl>();
        s.AddScoped<BehaviorStrategyService>();
        return s;
    }
}
