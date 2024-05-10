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
    public class QuestionMappingsRepository : IQuestionMappingsRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<QuestionMappings> _repository;

        public QuestionMappingsRepository(IDistributedCache distributedCache, IRepositoryAsync<QuestionMappings> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<QuestionMappings> QuestionMappings => _repository.Entities;

        public async Task<List<QuestionMappings>> GetListAsync()
        {
            try
            {
                return await _repository.Entities.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<QuestionMappings> GetByIdAsync(int id)
        {
            return await _repository.Entities.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> InsertAsync(QuestionMappings questionMapping)
        {
            await _repository.AddAsync(questionMapping);
            await _distributedCache.RemoveAsync(QuestionMappingsCacheKeys.ListKey);
            return questionMapping.Id;
        }

        public async Task DeleteAsync(QuestionMappings questionMapping)
        {
            await _repository.DeleteAsync(questionMapping);
            await _distributedCache.RemoveAsync(QuestionMappingsCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(QuestionMappingsCacheKeys.GetKey(questionMapping.Id));
        }

        public async Task UpdateAsync(QuestionMappings questionMappings)
        {
            await _repository.UpdateAsync(questionMappings);
            await _distributedCache.RemoveAsync(QuestionMappingsCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(QuestionMappingsCacheKeys.GetKey(questionMappings.Id));
        }

        public async Task<List<QuestionMappings>> GetByCampaignIdAsync(int campaignId)
        {
            try
            {
                var result = await _repository.Entities.Where(q => q.CampaignId == campaignId && q.IsDeleted == false).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
