using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.QuestionCategory.Query.GetAllCached
{
    public class GetAllQuestionCategoryCachedQuery : IRequest<Result<List<GetAllQuestionCategoryCachedResponse>>>
    {
    }

    public class
        GetAllQuestionCategoryCachedQueryHandler : IRequestHandler<GetAllQuestionCategoryCachedQuery,
        Result<List<GetAllQuestionCategoryCachedResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionCategoryCacheRepository _questionCategoryCache;

        public GetAllQuestionCategoryCachedQueryHandler(IQuestionCategoryCacheRepository questionCategoryCache, IMapper mapper)
        {
            _questionCategoryCache = questionCategoryCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllQuestionCategoryCachedResponse>>> Handle(GetAllQuestionCategoryCachedQuery request,
            CancellationToken cancellationToken)
        {
            var questionList = await _questionCategoryCache.GetCachedListAsync();
            //storeList = storeList.Where(x => !x.IsDeleted).ToList();

            var mappedQuestions = _mapper.Map<List<GetAllQuestionCategoryCachedResponse>>(questionList);
            return Result<List<GetAllQuestionCategoryCachedResponse>>.Success(mappedQuestions);
        }
    }
}
