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
    public class CampaignCacheRepository : ICampaignCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ICampaignRepository _campaignRepository;

        public CampaignCacheRepository(IDistributedCache distributedCache, ICampaignRepository campaignRepository) 
        {
            _distributedCache = distributedCache;
            _campaignRepository = campaignRepository;
        }

        public async Task<Campaign> GetByIdAsync(int campaignId)
        {
            var cacheKey = CampaignCacheKeys.GetKey(campaignId);
            var campaign = await _distributedCache.GetAsync<Campaign>(cacheKey);
            if (campaign == null)
            {
                campaign = await _campaignRepository.GetByIdAsync(campaignId);
                Throw.Exception.IfNull(campaign, "Campaign", "No Campaign Found");
                await _distributedCache.SetAsync(cacheKey, campaign);
            }

            return campaign;
        }

        public async Task<List<Campaign>> GetCachedListAsync()
        {
            var cacheKey = CampaignCacheKeys.ListKey;
            var campaignList = await _distributedCache.GetAsync<List<Campaign>>(cacheKey);
            if (campaignList == null)
            {
                campaignList = await _campaignRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, campaignList);
            }

            return campaignList;
        }
    }
}
