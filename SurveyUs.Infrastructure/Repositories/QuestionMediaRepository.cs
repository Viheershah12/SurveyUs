using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Entities;
using SurveyUs.Infrastructure.CacheKeys;

namespace SurveyUs.Infrastructure.Repositories
{
    public class QuestionMediaRepository : IQuestionMediaRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<QuestionMedia> _questionMediaRepository;

        public QuestionMediaRepository(
            IDistributedCache distributedCache,
            IRepositoryAsync<QuestionMedia> questionMediaRepository)
        {
            _distributedCache = distributedCache;
            _questionMediaRepository = questionMediaRepository;
        }

        public IQueryable<QuestionMedia> QuestionMedia => _questionMediaRepository.Entities;

        public async Task<List<QuestionMedia>> GetListAsync()
        {
            try
            {
                return await _questionMediaRepository.Entities.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<QuestionMedia> GetByIdAsync(int id)
        {
            return await _questionMediaRepository.Entities.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> InsertAsync(QuestionMedia questionMedia)
        {
            try
            {
                await _questionMediaRepository.AddAsync(questionMedia);
                await _distributedCache.RemoveAsync(QuestionMediaCacheKeys.ListKey);

                return questionMedia.Id;
            }
            catch (Exception ex)
            {
                throw new ApplicationException();
            }
        }

        public async Task<int> UpdateAsync(QuestionMedia questionMedia)
        {
            var cacheKey = QuestionMediaCacheKeys.GetKey(questionMedia.Id);

            await _questionMediaRepository.UpdateAsync(questionMedia);
            await _distributedCache.RemoveAsync(QuestionMediaCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(cacheKey);

            return questionMedia.Id;
        }

        public async Task<List<QuestionMedia>> GetByQuestionId(int questionId, int campaignId, int storeId, string userId)
        {
            return await _questionMediaRepository.Entities.Where(pm => 
                    pm.QuestionId == questionId && pm.CampaignId == campaignId && pm.StoreId == storeId && pm.UserId == userId)
                .ToListAsync();
        }
    }
}
