using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.CampaignMappings.Queries.GetAllCachedId
{
    public class GetAllCampaignMappingsCachedQuery : IRequest<Result<List<GetAllCampaignMappingsCachedResponse>>>
    {
    }

    public class
        GetAllCampaignMappingsCachedQueryHandler : IRequestHandler<GetAllCampaignMappingsCachedQuery,
        Result<List<GetAllCampaignMappingsCachedResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly ICampaignMappingsCacheRepository _campaignMappingCache;

        public GetAllCampaignMappingsCachedQueryHandler(ICampaignMappingsCacheRepository campaignMappingCache, IMapper mapper)
        {
            _campaignMappingCache = campaignMappingCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllCampaignMappingsCachedResponse>>> Handle(GetAllCampaignMappingsCachedQuery request,
            CancellationToken cancellationToken)
        {
            var campaignMappingsList = await _campaignMappingCache.GetCachedListAsync();
            campaignMappingsList = campaignMappingsList.Where(x => !x.IsDeleted).ToList();

            var mappedCampaignMappings = _mapper.Map<List<GetAllCampaignMappingsCachedResponse>>(campaignMappingsList);
            return Result<List<GetAllCampaignMappingsCachedResponse>>.Success(mappedCampaignMappings);
        }
    }
}
