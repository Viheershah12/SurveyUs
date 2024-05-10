using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.UserExtension.Queries.GetAllCached
{
    public class GetAllUserExtensionCachedQuery : IRequest<Result<List<GetAllUserExtensionCachedResponse>>>
    {
    }

    public class GetAllUserExtensionCachedQueryHandler : IRequestHandler<GetAllUserExtensionCachedQuery,
        Result<List<GetAllUserExtensionCachedResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IUserExtensionCacheRepository _userExtensionCache;

        public GetAllUserExtensionCachedQueryHandler(IUserExtensionCacheRepository userExtensionCache, IMapper mapper)
        {
            _userExtensionCache = userExtensionCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllUserExtensionCachedResponse>>> Handle(
            GetAllUserExtensionCachedQuery request, CancellationToken cancellationToken)
        {
            var userExtensionList = await _userExtensionCache.GetCachedListAsync();
            var mappedUserExtension = _mapper.Map<List<GetAllUserExtensionCachedResponse>>(userExtensionList);
            return Result<List<GetAllUserExtensionCachedResponse>>.Success(mappedUserExtension);
        }
    }
}