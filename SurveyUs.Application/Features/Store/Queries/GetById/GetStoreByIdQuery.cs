using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.Store.Queries.GetById
{
    public class GetStoreByIdQuery : IRequest<Result<GetStoreByIdResponse>>
    {
        public int Id { get; set; }

        public class GetProductByIdQueryHandler : IRequestHandler<GetStoreByIdQuery, Result<GetStoreByIdResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IStoreCacheRepository _storeCache;

            public GetProductByIdQueryHandler(IStoreCacheRepository productCache, IMapper mapper)
            {
                _storeCache = productCache;
                _mapper = mapper;
            }

            public async Task<Result<GetStoreByIdResponse>> Handle(GetStoreByIdQuery query,
                CancellationToken cancellationToken)
            {
                var store = await _storeCache.GetByIdAsync(query.Id);
                var mappedStore = _mapper.Map<GetStoreByIdResponse>(store);
                return Result<GetStoreByIdResponse>.Success(mappedStore);
            }
        }
    }
}