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
    public class QuestionMediaCacheRepository : IQuestionMediaCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQuestionMediaRepository _questionMediaRepository;

        public QuestionMediaCacheRepository(
            IDistributedCache distributedCache,
            IQuestionMediaRepository questionMediaRepository
        )
        {
            _distributedCache = distributedCache;
            _questionMediaRepository = questionMediaRepository;
        }

        public async Task<List<QuestionMedia>> GetCachedListAsync()
        {
            var cacheKey = QuestionCacheKeys.ListKey;
            var questionList = await _distributedCache.GetAsync<List<QuestionMedia>>(cacheKey);
            if (questionList == null)
            {
                questionList = await _questionMediaRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, questionList);
            }
            return questionList;
        }
        public async Task<QuestionMedia> GetCachedByIdAsync(int id)
        {
            var cacheKey = QuestionMediaCacheKeys.GetKey(id);
            var question = await _distributedCache.GetAsync<QuestionMedia>(cacheKey);
            if (question == null)
            {
                question = await _questionMediaRepository.GetByIdAsync(id);
                Throw.Exception.IfNull(question, "Question", "No Question Found");
                await _distributedCache.SetAsync(cacheKey, question);
            }

            return question;
        }

        public async Task<bool> SaveQuestionFileToCache(List<QuestionMedia> questionMedia, int storeId, int campaignId, string userId, int pageNumber)
        {
            string cacheKey = UserAnswersCacheKeys.GetFileSaveKey(storeId, campaignId, userId, pageNumber);

            await _distributedCache.RemoveAsync(cacheKey);
            await _distributedCache.SetAsync(cacheKey, questionMedia);

            return true;
        }

        public async Task<List<QuestionMedia>> GetSavedQuestionFileByPageNumber(int storeId, int campaignId, string userId, int pageNumber)
        {
            string cacheKey = UserAnswersCacheKeys.GetFileSaveKey(storeId, campaignId, userId, pageNumber);
            var savedMedia = await _distributedCache.GetAsync<List<QuestionMedia>>(cacheKey);

            return savedMedia;
        }
    }
}
