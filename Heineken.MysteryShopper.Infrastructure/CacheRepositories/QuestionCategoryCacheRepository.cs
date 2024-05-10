using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using Microsoft.Extensions.Caching.Distributed;
using SurveyUs.Application.Interfaces.CacheRepositories;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Entities;
using SurveyUs.Infrastructure.CacheKeys;

namespace SurveyUs.Infrastructure.CacheRepositories
{
    public class QuestionCategoryCacheRepository : IQuestionCategoryCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQuestionCategoryRepository _questionCategoryRepository;

        public QuestionCategoryCacheRepository(IDistributedCache distributedCache, IQuestionCategoryRepository questionCategoryRepository)
        {
            _distributedCache = distributedCache;
            _questionCategoryRepository = questionCategoryRepository;
        }

        public async Task<List<QuestionCategory>> GetCachedListAsync()
        {
            var cacheKey = QuestionCategoryCacheKeys.ListKey;
            var questionList = await _distributedCache.GetAsync<List<QuestionCategory>>(cacheKey);
            if (questionList == null)
            {
                questionList = await _questionCategoryRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, questionList);
            }
            return questionList;
        }


        public async Task<QuestionCategory> GetCachedByIdAsync(int id)
        {
            var cacheKey = QuestionCategoryCacheKeys.GetKey(id);
            var question = await _distributedCache.GetAsync<QuestionCategory>(cacheKey);
            if (question == null)
            {
                question = await _questionCategoryRepository.GetByIdAsync(id);
                Throw.Exception.IfNull(question, "QuestionCategory", "No QuestionCategory Found");
                await _distributedCache.SetAsync(cacheKey, question);
            }

            return question;
        }
    }
}
