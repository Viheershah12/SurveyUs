using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface ICampaignMappingsRepository
    {
        IQueryable<CampaignMappings> CampaignMappings { get; }

        Task<List<CampaignMappings>> GetListAsync();

        Task<CampaignMappings> GetByIdAsync(int campaignMappingId);

        Task<List<CampaignMappings>> GetByStoreIdAsync(int storeId);

        Task<int> InsertAsync(CampaignMappings campaignMappings);

        Task UpdateAsync(CampaignMappings campaignMappings);

        Task DeleteAsync(CampaignMappings campaignMappings);
    }
}
