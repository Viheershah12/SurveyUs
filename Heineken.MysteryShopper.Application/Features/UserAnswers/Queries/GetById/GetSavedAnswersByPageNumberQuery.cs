using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.UserAnswers.Queries.GetById
{
    public class GetSavedAnswersByPageNumberQuery : IRequest<Result<List<GetUserAnswersByIdResponse>>>
    {
        public int StoreId { get; set; }
        public int CampaignId { get; set; }
        public int PageNumber { get; set;}
        public string UserId { get; set; }

        public class GetSavedAnswersByPageNumberQeuryHandler : IRequestHandler<GetSavedAnswersByPageNumberQuery, Result<List<GetUserAnswersByIdResponse>>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAnswersCacheRepository _userAnswersCache;

            public GetSavedAnswersByPageNumberQeuryHandler(IUserAnswersCacheRepository userAnswersCache, IMapper mapper)
            {
                _userAnswersCache = userAnswersCache;
                _mapper = mapper;
            }
            public async Task<Result<List<GetUserAnswersByIdResponse>>> Handle(GetSavedAnswersByPageNumberQuery query,
                CancellationToken cancellationToken)
            {
                var question = await _userAnswersCache.GetSavedAnswersByPageNumber(query.StoreId, query.CampaignId, query.UserId, query.PageNumber);
                var mappedQuestion = _mapper.Map<List<GetUserAnswersByIdResponse>>(question);
                return Result<List<GetUserAnswersByIdResponse>>.Success(mappedQuestion);
            }

        }
    }
}
