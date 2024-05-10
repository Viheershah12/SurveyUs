using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.CacheRepositories
{
    public interface IStoreMappingsCacheRepository
    {
        Task<List<StoreMappings>> GetCachedListAsync();

        Task<StoreMappings> GetByIdAsync(int storeMappingId);
    }
}
