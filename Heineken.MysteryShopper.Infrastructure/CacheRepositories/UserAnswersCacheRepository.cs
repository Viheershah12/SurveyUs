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
    public class UserAnswersCacheRepository : IUserAnswersCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IUserAnswersRepository _userAnswersRepository;

        public UserAnswersCacheRepository(IDistributedCache distributedCache, IUserAnswersRepository userAnswersRepository)
        {
            _distributedCache = distributedCache;
            _userAnswersRepository = userAnswersRepository;
        }

        public async Task<List<UserAnswers>> GetCachedListAsync()
        {
            var cacheKey = UserAnswersCacheKeys.ListKey;
            var userAnswersList = await _distributedCache.GetAsync<List<UserAnswers>>(cacheKey);
            if (userAnswersList == null)
            {
                userAnswersList = await _userAnswersRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, userAnswersList);
            }
            return userAnswersList;
        }

        public async Task<UserAnswers> GetCachedByIdAsync(int id)
        {
            var cacheKey = UserAnswersCacheKeys.GetKey(id);
            var userAnswers = await _distributedCache.GetAsync<UserAnswers>(cacheKey);
            if (userAnswers == null)
            {
                userAnswers = await _userAnswersRepository.GetByIdAsync(id);
                Throw.Exception.IfNull(userAnswers, "UserAnswers", "No UserAnswers Found");
                await _distributedCache.SetAsync(cacheKey, userAnswers);
            }

            return userAnswers;
        }

        public async Task<bool> InsertSavedAnswersRangeToCache(List<UserAnswers> answers, int storeId, int campaignId, string userId, int pageNumber)
        {
            string cacheKey = UserAnswersCacheKeys.GetSaveKey(storeId, campaignId, userId, pageNumber);

            await _distributedCache.RemoveAsync(cacheKey);
            await _distributedCache.SetAsync(cacheKey, answers);
            return true;
        }

        public async Task<List<UserAnswers>> GetSavedAnswersByPageNumber(int storeId, int campaignId, string userId, int pageNumber)
        {
            string cacheKey = UserAnswersCacheKeys.GetSaveKey(storeId, campaignId, userId, pageNumber);
            var savedAnswers = await _distributedCache.GetAsync<List<UserAnswers>>(cacheKey);
            Throw.Exception.IfNull(savedAnswers, "SavedAnswers", "No Saved Answers Found");
            
            return savedAnswers;
        }

        public async Task<bool> DeleteSavedAnswers(int storeId, int campaignId, string userId, int totalPage)
        {
            for (int i = 1; i <= totalPage; i++)
            {
                string cacheKey = UserAnswersCacheKeys.GetSaveKey(storeId, campaignId, userId, i);
                await _distributedCache.RemoveAsync(cacheKey);
            }

            return true;
        }
    }
}
