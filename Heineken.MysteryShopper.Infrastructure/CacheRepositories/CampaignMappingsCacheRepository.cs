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
    public class CampaignMappingsCacheRepository : ICampaignMappingsCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ICampaignMappingsRepository _campaignMappingsRepository;

        public CampaignMappingsCacheRepository(IDistributedCache distributedCache, ICampaignMappingsRepository campaignMappingsRepository) 
        {
            _distributedCache = distributedCache;
            _campaignMappingsRepository = campaignMappingsRepository;
        }

        public async Task<List<CampaignMappings>> GetByStoreIdAsync(int storeId)
        {
            var campaign = await _campaignMappingsRepository.GetByStoreIdAsync(storeId);
            Throw.Exception.IfNull(campaign, "Campaign", "No Campaign Found For Store");

            return campaign;
        }

        public async Task<CampaignMappings> GetByIdAsync(int campaignMappingId)
        {
            var cacheKey = CampaignMappingsCacheKeys.GetKey(campaignMappingId);
            var campaignMapping = await _distributedCache.GetAsync<CampaignMappings>(cacheKey);
            if (campaignMapping == null)
            {
                campaignMapping = await _campaignMappingsRepository.GetByIdAsync(campaignMappingId);
                Throw.Exception.IfNull(campaignMapping, "CampaignMappings", "No CampaignMapping Found");
                await _distributedCache.SetAsync(cacheKey, campaignMapping);
            }

            return campaignMapping;
        }

        public async Task<List<CampaignMappings>> GetCachedListAsync()
        {
            var cacheKey = CampaignCacheKeys.ListKey;
            var campaignList = await _distributedCache.GetAsync<List<CampaignMappings>>(cacheKey);
            if (campaignList == null)
            {
                campaignList = await _campaignMappingsRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, campaignList);
            }

            return campaignList;
        }
    }
}
