using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.UserExtension.Queries.GetById
{
    public class GetUserExtensionByIdQuery : IRequest<Result<GetUserExtensionByIdResponse>>
    {
        public int Id { get; set; }

        public class
            GetUserExtensionByIdQueryHandler : IRequestHandler<GetUserExtensionByIdQuery,
            Result<GetUserExtensionByIdResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IUserExtensionCacheRepository _userExtensionCache;

            public GetUserExtensionByIdQueryHandler(IUserExtensionCacheRepository userExtensionCache, IMapper mapper)
            {
                _userExtensionCache = userExtensionCache;
                _mapper = mapper;
            }

            public async Task<Result<GetUserExtensionByIdResponse>> Handle(GetUserExtensionByIdQuery query,
                CancellationToken cancellationToken)
            {
                var userExtension = await _userExtensionCache.GetByIdAsync(query.Id);
                var mappedUserExtension = _mapper.Map<GetUserExtensionByIdResponse>(userExtension);
                return Result<GetUserExtensionByIdResponse>.Success(mappedUserExtension);
            }
        }
    }
}