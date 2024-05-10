using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.QuestionChoices.Queries.GetById
{
    public class GetQuestionChoicesByIdQuery : IRequest<Result<GetQuestionChoicesByIdResponse>>
    {
        public int Id { get; set; }

        public class GetQuestionChoicesByIdQueryHandler : IRequestHandler<GetQuestionChoicesByIdQuery,
            Result<GetQuestionChoicesByIdResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IQuestionChoicesCacheRepository _questionChoicesCache;

            public GetQuestionChoicesByIdQueryHandler(IQuestionChoicesCacheRepository questionChoicesCache,
                IMapper mapper)
            {
                _questionChoicesCache = questionChoicesCache;
                _mapper = mapper;
            }

            public async Task<Result<GetQuestionChoicesByIdResponse>> Handle(GetQuestionChoicesByIdQuery query,
                CancellationToken cancellationToken)
            {
                var question = await _questionChoicesCache.GetCachedByIdAsync(query.Id);
                var mappedQuestion = _mapper.Map<GetQuestionChoicesByIdResponse>(question);
                return Result<GetQuestionChoicesByIdResponse>.Success(mappedQuestion);
            }
        }
    }

}
