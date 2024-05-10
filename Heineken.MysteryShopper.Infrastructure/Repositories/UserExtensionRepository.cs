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
    public class UserExtensionRepository : IUserExtensionRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<UserExtension> _repository;

        public UserExtensionRepository(IDistributedCache distributedCache, IRepositoryAsync<UserExtension> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<UserExtension> UserExtension => _repository.Entities;

        public async Task DeleteAsync(UserExtension userExtension)
        {
            await _repository.DeleteAsync(userExtension);
            await _distributedCache.RemoveAsync(UserExtensionCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(UserExtensionCacheKeys.GetKey(userExtension.Id));
        }

        public async Task<UserExtension> GetByIdAsync(int userExtensionId)
        {
            return await _repository.Entities.Where(p => p.Id == userExtensionId).FirstOrDefaultAsync();
        }

        public async Task<List<UserExtension>> GetListAsync()
        {
            return await _repository.Entities.ToListAsync();
        }

        public async Task<int> InsertAsync(UserExtension userExtension)
        {
            await _repository.AddAsync(userExtension);
            await _distributedCache.RemoveAsync(UserExtensionCacheKeys.ListKey);
            return userExtension.Id;
        }

        public async Task UpdateAsync(UserExtension userExtension)
        {
            await _repository.UpdateAsync(userExtension);
            await _distributedCache.RemoveAsync(UserExtensionCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(UserExtensionCacheKeys.GetKey(userExtension.Id));
        }

        public async Task<UserExtension> GetByEmailAsync(string email)
        {
            return await _repository.Entities.Where(p => p.Email == email).FirstOrDefaultAsync();
        }

        public async Task<List<UserExtension>> GetByStoreAsync(int storeId)
        {
            return await _repository.Entities.Where(p => p.Store == storeId).ToListAsync();
        }

        public async Task<UserExtension> GetByUserIdAsync(string userId)
        {
            return await _repository.Entities.Where(p => p.UserId == userId).FirstOrDefaultAsync();
        }
    }
}