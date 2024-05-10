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
    public class StoreCacheRepository : IStoreCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IStoreRepository _storeRepository;

        public StoreCacheRepository(IDistributedCache distributedCache, IStoreRepository storeRepository)
        {
            _distributedCache = distributedCache;
            _storeRepository = storeRepository;
        }

        public async Task<Store> GetByIdAsync(int storeId)
        {
            var cacheKey = StoreCacheKeys.GetKey(storeId);
            var store = await _distributedCache.GetAsync<Store>(cacheKey);
            if (store == null)
            {
                store = await _storeRepository.GetByIdAsync(storeId);
                Throw.Exception.IfNull(store, "Store", "No Store Found");
                await _distributedCache.SetAsync(cacheKey, store);
            }

            return store;
        }

        public async Task<List<Store>> GetCachedListAsync()
        {
            var cacheKey = StoreCacheKeys.ListKey;
            var storeList = await _distributedCache.GetAsync<List<Store>>(cacheKey);
            if (storeList == null)
            {
                try
                {
                    storeList = await _storeRepository.GetListAsync();
                    await _distributedCache.SetAsync(cacheKey, storeList);
                }
                catch
                {

                }
            }

            return storeList;
        }
    }
}