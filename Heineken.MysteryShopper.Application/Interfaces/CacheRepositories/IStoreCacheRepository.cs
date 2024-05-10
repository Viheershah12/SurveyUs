using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.CacheRepositories
{
    public interface IStoreCacheRepository
    {
        Task<List<Store>> GetCachedListAsync();

        Task<Store> GetByIdAsync(int storeId);
    }
}