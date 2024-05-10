using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.Campaign.Queries.GetById
{
    public class GetCampaignByIdQuery : IRequest<Result<GetCampaignByIdResponse>>
    {
        public int Id { get; set; }

        public class GetCampaignByIdQueryHandler : IRequestHandler<GetCampaignByIdQuery, Result<GetCampaignByIdResponse>>
        {
            private readonly IMapper _mapper;
            private readonly ICampaignCacheRepository _campaignCache;

            public GetCampaignByIdQueryHandler(ICampaignCacheRepository campaignCache, IMapper mapper)
            {
                _mapper = mapper;
                _campaignCache = campaignCache;
            }

            public async Task<Result<GetCampaignByIdResponse>> Handle(GetCampaignByIdQuery request, CancellationToken cancellationToken)
            {
                var campaign = await _campaignCache.GetByIdAsync(request.Id);
                var mappedCampaign = _mapper.Map<GetCampaignByIdResponse>(campaign);
                return Result<GetCampaignByIdResponse>.Success(mappedCampaign);
            }
        }
    }


}
