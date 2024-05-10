using Microsoft.Extensions.Caching.Distributed;
using SurveyUs.Application.Interfaces.CacheRepositories;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Infrastructure.CacheRepositories
{
    public class QuestionAnswerCacheRepository : IQuestionAnswerCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQuestionAnswerRepository _questionAnswerRepository;

        public QuestionAnswerCacheRepository(
            IDistributedCache distributedCache,
            IQuestionAnswerRepository questionAnswerRepository
        )
        {
            _distributedCache = distributedCache;
            _questionAnswerRepository = questionAnswerRepository;
        }


    }
}
