using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.Question.Queries.GetById
{
    public class GetQuestionByIdQuery : IRequest<Result<GetQuestionByIdResponse>>
    {
        public int Id { get; set; }

        public class GetQuestionByIdQueryHandler : IRequestHandler<GetQuestionByIdQuery, Result<GetQuestionByIdResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IQuestionCacheRepository _questionCache;

            public GetQuestionByIdQueryHandler(IQuestionCacheRepository questionCache, IMapper mapper)
            {
                _questionCache = questionCache;
                _mapper = mapper;
            }

            public async Task<Result<GetQuestionByIdResponse>> Handle(GetQuestionByIdQuery query,
                CancellationToken cancellationToken)
            {
                var question = await _questionCache.GetCachedByIdAsync(query.Id);
                var mappedQuestion = _mapper.Map<GetQuestionByIdResponse>(question);
                return Result<GetQuestionByIdResponse>.Success(mappedQuestion);
            }
        }
    }
}
