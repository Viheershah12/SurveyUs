using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.Store.Queries.GetAllCached
{
    public class GetAllStoresCachedQuery : IRequest<Result<List<GetAllStoresCachedResponse>>>
    {
    }

    public class
        GetAllStoresCachedQueryHandler : IRequestHandler<GetAllStoresCachedQuery,
        Result<List<GetAllStoresCachedResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IStoreCacheRepository _storeCache;

        public GetAllStoresCachedQueryHandler(IStoreCacheRepository storeCache, IMapper mapper)
        {
            _storeCache = storeCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllStoresCachedResponse>>> Handle(GetAllStoresCachedQuery request,
            CancellationToken cancellationToken)
        {
            var storeList = await _storeCache.GetCachedListAsync();
            storeList = storeList.Where(x => !x.IsDeleted).ToList();

            var mappedStores = _mapper.Map<List<GetAllStoresCachedResponse>>(storeList);
            return Result<List<GetAllStoresCachedResponse>>.Success(mappedStores);
        }
    }
}