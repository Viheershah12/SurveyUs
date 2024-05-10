using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.StoreMappings.Queries.GetById
{
    public class GetStoreMappingsByIdQuery : IRequest<Result<GetStoreMappingsByIdResponse>>
    {
        public int Id { get; set; }

        public class GetStoreMappingsByIdQueryHandler : IRequestHandler<GetStoreMappingsByIdQuery, Result<GetStoreMappingsByIdResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IStoreMappingsCacheRepository _storeMappingsCache;

            public GetStoreMappingsByIdQueryHandler(IStoreMappingsCacheRepository storeMappingsCache, IMapper mapper)
            {
                _storeMappingsCache = storeMappingsCache;
                _mapper = mapper;
            }

            public async Task<Result<GetStoreMappingsByIdResponse>> Handle(GetStoreMappingsByIdQuery query,
                CancellationToken cancellationToken)
            {
                var storeMappings = await _storeMappingsCache.GetByIdAsync(query.Id);
                var mappedStoreMapping = _mapper.Map<GetStoreMappingsByIdResponse>(storeMappings);
                return Result<GetStoreMappingsByIdResponse>.Success(mappedStoreMapping);
            }
        }
    }
}
