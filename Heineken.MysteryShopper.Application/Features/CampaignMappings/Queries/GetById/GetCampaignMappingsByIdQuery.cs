using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.CampaignMappings.Queries.GetById
{
    public class GetCampaignMappingsByIdQuery : IRequest<Result<GetCampaignMappingsByIdResponse>>
    {
        public int Id { get; set; }

        public class GetCampaignMappingsByIdQueryHandler : IRequestHandler<GetCampaignMappingsByIdQuery, Result<GetCampaignMappingsByIdResponse>>
        {
            private readonly IMapper _mapper;
            private readonly ICampaignMappingsCacheRepository _campaignMappingsCache;

            public GetCampaignMappingsByIdQueryHandler(ICampaignMappingsCacheRepository campaignMappingsCache, IMapper mapper)
            {
                _campaignMappingsCache = campaignMappingsCache;
                _mapper = mapper;
            }

            public async Task<Result<GetCampaignMappingsByIdResponse>> Handle(GetCampaignMappingsByIdQuery query,
                CancellationToken cancellationToken)
            {
                var campaignMappings = await _campaignMappingsCache.GetByIdAsync(query.Id);
                var mappedCampaignMapping = _mapper.Map<GetCampaignMappingsByIdResponse>(campaignMappings);
                return Result<GetCampaignMappingsByIdResponse>.Success(mappedCampaignMapping);
            }
        }
    }
}
