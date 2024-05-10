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
    public class StoreMappingsRepository : IStoreMappingsRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<StoreMappings> _storeMappingsRepository;

        public StoreMappingsRepository(IDistributedCache distributedCache, IRepositoryAsync<StoreMappings> storeMappingsRepository)
        {
            _distributedCache = distributedCache;
            _storeMappingsRepository = storeMappingsRepository;
        }

        public IQueryable<StoreMappings> StoreMappings => _storeMappingsRepository.Entities;

        public async Task DeleteAsync(StoreMappings storeMapping)
        {
            await _storeMappingsRepository.DeleteAsync(storeMapping);
            await _distributedCache.RemoveAsync(StoreCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(StoreCacheKeys.GetKey(storeMapping.Id));
        }

        public async Task<StoreMappings> GetByIdAsync(int storeMappingId)
        {
            return await _storeMappingsRepository.Entities.Where(p => p.Id == storeMappingId).FirstOrDefaultAsync();
        }

        public async Task<List<StoreMappings>> GetListAsync()
        {
            return await _storeMappingsRepository.Entities.ToListAsync();
        }

        public async Task<List<StoreMappings>> GetUsersByStoreIdAsync(int storeId)
        {
            try
            {
                var result = await _storeMappingsRepository.Entities.Where(s => s.StoreId == storeId && s.IsDeleted == false).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> InsertAsync(StoreMappings storeMapping)
        {
            await _storeMappingsRepository.AddAsync(storeMapping);
            await _distributedCache.RemoveAsync(StoreMappingsCacheKeys.ListKey);
            return storeMapping.Id;
        }

        public async Task UpdateAsync(StoreMappings storeMapping)
        {
            await _storeMappingsRepository.UpdateAsync(storeMapping);
            await _distributedCache.RemoveAsync(StoreMappingsCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(StoreMappingsCacheKeys.GetKey(storeMapping.Id));
        }
    }
}
