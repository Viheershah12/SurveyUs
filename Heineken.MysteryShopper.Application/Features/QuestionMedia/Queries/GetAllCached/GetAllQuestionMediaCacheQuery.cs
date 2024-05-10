using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.QuestionMedia.Queries.GetAllCached
{
    public class GetAllQuestionMediaCacheQuery : IRequest<Result<List<GetAllQuestionMediaCacheResponse>>>
    {
    }

    public class GetAllQuestionMediaCacheQueryHandler : IRequestHandler<GetAllQuestionMediaCacheQuery, Result<List<GetAllQuestionMediaCacheResponse>>>
    {
        private readonly IMapper _mapper;
        private IQuestionMediaCacheRepository _questionMediaCacheRepository;

        public async Task<Result<List<GetAllQuestionMediaCacheResponse>>> Handle(GetAllQuestionMediaCacheQuery request, CancellationToken cancellationToken)
        {
            var questionList = await _questionMediaCacheRepository.GetCachedListAsync();
            //storeList = storeList.Where(x => !x.IsDeleted).ToList();

            var mappedQuestions = _mapper.Map<List<GetAllQuestionMediaCacheResponse>>(questionList);
            return Result<List<GetAllQuestionMediaCacheResponse>>.Success(mappedQuestions);
        }
    }
}
