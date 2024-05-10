using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.QuestionCategoryMapping.Queries.GetById
{
    public class GetQuestionCategoryMappingByIdQuery : IRequest<Result<GetQuestionCategoryMappingByIdResponse>>
    {
        public int Id { get; set; }

        public class GetQuestionCategoryMappingByIdQueryHandler : IRequestHandler<GetQuestionCategoryMappingByIdQuery, Result<GetQuestionCategoryMappingByIdResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IQuestionCategoryMappingCacheRepository _questionCategoryMappingCache;

            public GetQuestionCategoryMappingByIdQueryHandler(IQuestionCategoryMappingCacheRepository questionCategoryMappingCache, IMapper mapper)
            {
                _questionCategoryMappingCache = questionCategoryMappingCache;
                _mapper = mapper;
            }

            public async Task<Result<GetQuestionCategoryMappingByIdResponse>> Handle(GetQuestionCategoryMappingByIdQuery query,
                CancellationToken cancellationToken)
            {
                var question = await _questionCategoryMappingCache.GetCachedByIdAsync(query.Id);
                var mappedQuestion = _mapper.Map<GetQuestionCategoryMappingByIdResponse>(question);
                return Result<GetQuestionCategoryMappingByIdResponse>.Success(mappedQuestion);
            }
        }
    }
}
