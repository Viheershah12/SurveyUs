using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.StoreMappings.Queries.GetAllCachedId
{
    public class GetAllStoreMappingsCachedQuery :IRequest<Result<List<GetAllStoreMappingsCachedResponse>>>
    {
    }

    public class
        GetAllStoreMappingsCachedQueryHandler : IRequestHandler<GetAllStoreMappingsCachedQuery,
        Result<List<GetAllStoreMappingsCachedResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IStoreMappingsCacheRepository _storeMappingCache;

        public GetAllStoreMappingsCachedQueryHandler(IStoreMappingsCacheRepository storeMappingCache, IMapper mapper)
        {
            _storeMappingCache = storeMappingCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllStoreMappingsCachedResponse>>> Handle(GetAllStoreMappingsCachedQuery request,
            CancellationToken cancellationToken)
        {
            var storeMappingsList = await _storeMappingCache.GetCachedListAsync();
            storeMappingsList = storeMappingsList.Where(x => !x.IsDeleted).ToList();

            var mappedStoreMappings = _mapper.Map<List<GetAllStoreMappingsCachedResponse>>(storeMappingsList);
            return Result<List<GetAllStoreMappingsCachedResponse>>.Success(mappedStoreMappings);
        }
    }
}
