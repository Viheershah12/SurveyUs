using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.QuestionChoices.Queries.GetAllCached
{
    public class GetAllQuestionChoicesCachedQuery : IRequest<Result<List<GetAllQuestionChoicesCachedResponse>>>
    {
    }

    public class
        GetAllQuestionChoicesCachedQueryHandler : IRequestHandler<GetAllQuestionChoicesCachedQuery,
        Result<List<GetAllQuestionChoicesCachedResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionChoicesCacheRepository _questionChoicesCache;

        public GetAllQuestionChoicesCachedQueryHandler(IQuestionChoicesCacheRepository questionChoicesCache, IMapper mapper)
        {
            _questionChoicesCache = questionChoicesCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllQuestionChoicesCachedResponse>>> Handle(GetAllQuestionChoicesCachedQuery request,
            CancellationToken cancellationToken)
        {
            var questionChoicesList = await _questionChoicesCache.GetCachedListAsync();
            //storeList = storeList.Where(x => !x.IsDeleted).ToList();

            var mappedQuestionChoices = _mapper.Map<List<GetAllQuestionChoicesCachedResponse>>(questionChoicesList);
            return Result<List<GetAllQuestionChoicesCachedResponse>>.Success(mappedQuestionChoices);
        }
    }
}
