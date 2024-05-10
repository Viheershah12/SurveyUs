using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.Question.Queries.GetAllCached
{
    public class GetAllQuestionCachedQuery : IRequest<Result<List<GetAllQuestionCachedResponse>>>
    {
    }

    public class
        GetAllQuestionCachedQueryHandler : IRequestHandler<GetAllQuestionCachedQuery,
        Result<List<GetAllQuestionCachedResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionCacheRepository _questionCache;

        public GetAllQuestionCachedQueryHandler(IQuestionCacheRepository storeCache, IMapper mapper)
        {
            _questionCache = storeCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllQuestionCachedResponse>>> Handle(GetAllQuestionCachedQuery request,
            CancellationToken cancellationToken)
        {
            var questionList = await _questionCache.GetCachedListAsync();
            //storeList = storeList.Where(x => !x.IsDeleted).ToList();

            var mappedQuestions = _mapper.Map<List<GetAllQuestionCachedResponse>>(questionList);
            return Result<List<GetAllQuestionCachedResponse>>.Success(mappedQuestions);
        }
    }
}
