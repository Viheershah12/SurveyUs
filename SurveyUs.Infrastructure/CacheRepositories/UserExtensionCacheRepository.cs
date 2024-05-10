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
    public class UserExtensionCacheRepository : IUserExtensionCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IUserExtensionRepository _userExtensionRepository;

        public UserExtensionCacheRepository(IDistributedCache distributedCache,
            IUserExtensionRepository userExtensionRepository)
        {
            _distributedCache = distributedCache;
            _userExtensionRepository = userExtensionRepository;
        }

        public async Task<UserExtension> GetByIdAsync(int userExtensionId)
        {
            var cacheKey = UserExtensionCacheKeys.GetKey(userExtensionId);
            var userExtension = await _distributedCache.GetAsync<UserExtension>(cacheKey);
            if (userExtension == null)
            {
                userExtension = await _userExtensionRepository.GetByIdAsync(userExtensionId);
                Throw.Exception.IfNull(userExtension, "UserExtension", "No UserExtension Found");
                await _distributedCache.SetAsync(cacheKey, userExtension);
            }

            return userExtension;
        }

        public async Task<List<UserExtension>> GetCachedListAsync()
        {
            var cacheKey = UserExtensionCacheKeys.ListKey;
            var userExtensionList = await _distributedCache.GetAsync<List<UserExtension>>(cacheKey);
            if (userExtensionList == null)
            {
                userExtensionList = await _userExtensionRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, userExtensionList);
            }

            return userExtensionList;
        }
    }
}