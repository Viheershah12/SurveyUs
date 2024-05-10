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
    public class CampaignRepository : ICampaignRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<Campaign> _repository;
        public CampaignRepository(IDistributedCache distributedCache, IRepositoryAsync<Campaign> repository) 
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<Campaign> Campaign => _repository.Entities;

        public async Task<List<Campaign>> GetListAsync()
        {
            try
            {
                return await _repository.Entities.Where(c => c.IsDeleted == false).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Campaign> GetByIdAsync(int campaignId)
        {
            return await _repository.Entities.Where(c => c.Id == campaignId && c.IsDeleted == false).FirstOrDefaultAsync();
        }

        public async Task<int> InsertAsync(Campaign campaign)
        {
            await _repository.AddAsync(campaign);
            await _distributedCache.RemoveAsync(CampaignCacheKeys.ListKey);
            return campaign.Id;
        }

        public async Task UpdateAsync(Campaign campaign)
        {
            await _repository.UpdateAsync(campaign);
            await _distributedCache.RemoveAsync(CampaignCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CampaignCacheKeys.GetKey(campaign.Id));
        }

        public async Task DeleteAsync(Campaign campaign)
        {
            await _repository.DeleteAsync(campaign);
            await _distributedCache.RemoveAsync(CampaignCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CampaignCacheKeys.GetKey(campaign.Id));
        }
    }
}
