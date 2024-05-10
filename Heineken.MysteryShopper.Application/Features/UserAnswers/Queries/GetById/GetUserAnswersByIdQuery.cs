using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.UserAnswers.Queries.GetById
{
    public class GetUserAnswersByIdQuery : IRequest<Result<GetUserAnswersByIdResponse>>
    {
        public int Id { get; set; }

        public class GetUserAnswersByIdQueryHandler : IRequestHandler<GetUserAnswersByIdQuery, Result<GetUserAnswersByIdResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAnswersCacheRepository _userAnswersCache;

            public GetUserAnswersByIdQueryHandler(IUserAnswersCacheRepository userAnswersCache, IMapper mapper)
            {
                _userAnswersCache = userAnswersCache;
                _mapper = mapper;
            }

            public async Task<Result<GetUserAnswersByIdResponse>> Handle(GetUserAnswersByIdQuery query,
                CancellationToken cancellationToken)
            {
                var question = await _userAnswersCache.GetCachedByIdAsync(query.Id);
                var mappedQuestion = _mapper.Map<GetUserAnswersByIdResponse>(question);
                return Result<GetUserAnswersByIdResponse>.Success(mappedQuestion);
            }
        }
    }
}
