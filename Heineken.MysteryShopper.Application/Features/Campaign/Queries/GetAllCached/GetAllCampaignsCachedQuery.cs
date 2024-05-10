using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.Campaign.Queries.GetAllCached
{
    public class GetAllCampaignsCachedQuery : IRequest<Result<List<GetAllCampaignsCachedResponse>>>
    {
    }

    public class GetAllCampaignsCachedQueryHandler : IRequestHandler<GetAllCampaignsCachedQuery, Result<List<GetAllCampaignsCachedResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly ICampaignCacheRepository _campaignCache;

        public GetAllCampaignsCachedQueryHandler(ICampaignCacheRepository campaignCache, IMapper mapper) 
        {
            _campaignCache = campaignCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllCampaignsCachedResponse>>> Handle(GetAllCampaignsCachedQuery request, CancellationToken cancellationToken)
        {
            var campaignList = await _campaignCache.GetCachedListAsync();
            var mappedCampaigns = _mapper.Map<List<GetAllCampaignsCachedResponse>>(campaignList);
            return Result<List<GetAllCampaignsCachedResponse>>.Success(mappedCampaigns);
        }
    }
}
