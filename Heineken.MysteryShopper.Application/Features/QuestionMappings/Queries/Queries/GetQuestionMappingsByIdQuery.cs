using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.QuestionMappings.Queries.Queries
{
    public class GetQuestionMappingsByIdQuery : IRequest<Result<GetQuestionMappingsByIdResponse>>
    {
        public int Id { get; set; }

        public class GetQuestionMappingsByIdHandler : IRequestHandler<GetQuestionMappingsByIdQuery, Result<GetQuestionMappingsByIdResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IQuestionMappingsCacheRepository _questionMappingsCache;

            public GetQuestionMappingsByIdHandler(IQuestionMappingsCacheRepository questionMappingsCache, IMapper mapper)
            {
                _questionMappingsCache = questionMappingsCache;
                _mapper = mapper;
            }

            public async Task<Result<GetQuestionMappingsByIdResponse>> Handle(GetQuestionMappingsByIdQuery query,
                CancellationToken cancellationToken)
            {
                var question = await _questionMappingsCache.GetCachedByIdAsync(query.Id);
                var mappedQuestion = _mapper.Map<GetQuestionMappingsByIdResponse>(question);
                return Result<GetQuestionMappingsByIdResponse>.Success(mappedQuestion);
            }
        }
    }
}
