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
    public class StoreRepository : IStoreRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<Store> _repository;

        public StoreRepository(IDistributedCache distributedCache, IRepositoryAsync<Store> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<Store> Store => _repository.Entities;

        public async Task DeleteAsync(Store store)
        {
            await _repository.DeleteAsync(store);
            await _distributedCache.RemoveAsync(StoreCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(StoreCacheKeys.GetKey(store.Id));
        }

        public async Task<Store> GetByIdAsync(int storeId)
        {
            return await _repository.Entities.Where(p => p.Id == storeId).FirstOrDefaultAsync();
        }

        public async Task<List<Store>> GetListAsync()
        {
            return await _repository.Entities.ToListAsync();
        }

        public async Task<int> InsertAsync(Store store)
        {
            await _repository.AddAsync(store);
            await _distributedCache.RemoveAsync(StoreCacheKeys.ListKey);
            return store.Id;
        }

        public async Task UpdateAsync(Store store)
        {
            await _repository.UpdateAsync(store);
            await _distributedCache.RemoveAsync(StoreCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(StoreCacheKeys.GetKey(store.Id));
        }

        public async Task<Store> GetByStateAsync(int state)
        {
            return await _repository.Entities.Where(p => (int)p.State == state).FirstOrDefaultAsync();
        }
    }
}