using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.QuestionCategory.Query.GetById
{
    public class GetQuestionCategoryByIdQuery : IRequest<Result<GetQuestionCategoryByIdResponse>>
    {
        public int Id { get; set; }

        public class GetQuestionCategoryByIdQueryHandler : IRequestHandler<GetQuestionCategoryByIdQuery, Result<GetQuestionCategoryByIdResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IQuestionCategoryCacheRepository _questionCategoryCache;

            public GetQuestionCategoryByIdQueryHandler(IQuestionCategoryCacheRepository questionCategoryCache, IMapper mapper)
            {
                _questionCategoryCache = questionCategoryCache;
                _mapper = mapper;
            }

            public async Task<Result<GetQuestionCategoryByIdResponse>> Handle(GetQuestionCategoryByIdQuery query,
                CancellationToken cancellationToken)
            {
                var question = await _questionCategoryCache.GetCachedByIdAsync(query.Id);
                var mappedQuestion = _mapper.Map<GetQuestionCategoryByIdResponse>(question);
                return Result<GetQuestionCategoryByIdResponse>.Success(mappedQuestion);
            }
        }
    }
}
