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
    public class CampaignMappingsRepository : ICampaignMappingsRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<CampaignMappings> _repository;
        public CampaignMappingsRepository(IDistributedCache distributedCache, IRepositoryAsync<CampaignMappings> repository) 
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<CampaignMappings> CampaignMappings => _repository.Entities;

        public async Task<List<CampaignMappings>> GetListAsync()
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

        public async Task<CampaignMappings> GetByIdAsync(int campaignMappingId)
        {
            try
            {
                return await _repository.Entities.Where(m => m.Id == campaignMappingId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<CampaignMappings>> GetByStoreIdAsync(int storeId)
        {
            try
            {
                var result = await _repository.Entities.Where(c => c.StoreId == storeId && c.IsDeleted == false).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> InsertAsync(CampaignMappings campaignMapping)
        {
            await _repository.AddAsync(campaignMapping);
            return campaignMapping.Id;
        }

        public async Task UpdateAsync(CampaignMappings campaignMappings)
        {
            await _repository.UpdateAsync(campaignMappings);
            await _distributedCache.RemoveAsync(CampaignMappingsCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CampaignMappingsCacheKeys.GetKey(campaignMappings.Id));
        }

        public async Task DeleteAsync(CampaignMappings campaignMappings)
        {
            await _repository.DeleteAsync(campaignMappings);
            await _distributedCache.RemoveAsync(CampaignMappingsCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CampaignMappingsCacheKeys.GetKey(campaignMappings.Id));
        }
    }
}
