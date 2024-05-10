using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.QuestionCategoryMapping.Queries.GetAllCached
{
    public class GetAllQuestionCategoryMappingCachedQuery : IRequest<Result<List<GetAllQuestionCategoryMappingCachedResponse>>>
    {
    }

    public class
        GetAllQuestionCategoryMappingCachedQueryHandler : IRequestHandler<GetAllQuestionCategoryMappingCachedQuery,
        Result<List<GetAllQuestionCategoryMappingCachedResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionCategoryMappingCacheRepository _questionCategoryMappingCache;

        public GetAllQuestionCategoryMappingCachedQueryHandler(IQuestionCategoryMappingCacheRepository questionCategoryMappingCache, IMapper mapper)
        {
            _questionCategoryMappingCache = questionCategoryMappingCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllQuestionCategoryMappingCachedResponse>>> Handle(GetAllQuestionCategoryMappingCachedQuery request,
            CancellationToken cancellationToken)
        {
            var questionList = await _questionCategoryMappingCache.GetCachedListAsync();
            //storeList = storeList.Where(x => !x.IsDeleted).ToList();

            var mappedQuestions = _mapper.Map<List<GetAllQuestionCategoryMappingCachedResponse>>(questionList);
            return Result<List<GetAllQuestionCategoryMappingCachedResponse>>.Success(mappedQuestions);
        }
    }
}
