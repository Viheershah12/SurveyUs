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
    public class StoreMappingsCacheRepository : IStoreMappingsCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IStoreMappingsRepository _storeMappingsRepository;

        public StoreMappingsCacheRepository(IDistributedCache distributedCache, IStoreMappingsRepository storeMappingsRepository)
        {
            _distributedCache = distributedCache;
            _storeMappingsRepository = storeMappingsRepository;
        }

        public async Task<StoreMappings> GetByIdAsync(int storeMappingId)
        {
            var cacheKey = StoreMappingsCacheKeys.GetKey(storeMappingId);
            var storeMapping = await _distributedCache.GetAsync<StoreMappings>(cacheKey);
            if (storeMapping == null)
            {
                storeMapping = await _storeMappingsRepository.GetByIdAsync(storeMappingId);
                Throw.Exception.IfNull(storeMapping, "StoreMapping", "No Store Found");
                await _distributedCache.SetAsync(cacheKey, storeMapping);
            }

            return storeMapping;
        }

        public async Task<List<StoreMappings>> GetCachedListAsync()
        {
            var cacheKey = StoreMappingsCacheKeys.ListKey;
            var storeMappingsList = await _distributedCache.GetAsync<List<StoreMappings>>(cacheKey);
            if (storeMappingsList == null)
            {
                try
                {
                    storeMappingsList = await _storeMappingsRepository.GetListAsync();
                    await _distributedCache.SetAsync(cacheKey, storeMappingsList);
                }
                catch
                {

                }
            }

            return storeMappingsList;
        }
    }
}
