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
    public class QuestionCategoryMappingCacheRepository : IQuestionCategoryMappingCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQuestionCategoryMappingRepository _questionCategoryMappingRepository;

        public QuestionCategoryMappingCacheRepository(IDistributedCache distributedCache, IQuestionCategoryMappingRepository questionCategoryMappingRepository)
        {
            _distributedCache = distributedCache;
            _questionCategoryMappingRepository = questionCategoryMappingRepository;
        }

        public async Task<List<QuestionCategoryMapping>> GetCachedListAsync()
        {
            var cacheKey = QuestionCategoryMappingCacheKeys.ListKey;
            var questionList = await _distributedCache.GetAsync<List<QuestionCategoryMapping>>(cacheKey);
            if (questionList == null)
            {
                questionList = await _questionCategoryMappingRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, questionList);
            }
            return questionList;
        }
        
        public async Task<QuestionCategoryMapping> GetCachedByIdAsync(int id)
        {
            var cacheKey = QuestionCategoryMappingCacheKeys.GetKey(id);
            var question = await _distributedCache.GetAsync<QuestionCategoryMapping>(cacheKey);
            if (question == null)
            {
                question = await _questionCategoryMappingRepository.GetByIdAsync(id);
                Throw.Exception.IfNull(question, "QuestionCategoryMapping", "No QuestionCategoryMapping Found");
                await _distributedCache.SetAsync(cacheKey, question);
            }

            return question;
        }
    }
}
