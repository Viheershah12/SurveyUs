using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.CacheRepositories
{
    public interface ICampaignMappingsCacheRepository
    {
        Task<List<CampaignMappings>> GetCachedListAsync();
        Task<CampaignMappings> GetByIdAsync(int id);

        Task<List<CampaignMappings>> GetByStoreIdAsync(int storeId);
    }
}
