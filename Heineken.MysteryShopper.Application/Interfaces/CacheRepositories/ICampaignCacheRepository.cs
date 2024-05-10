using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.CacheRepositories
{
    public interface ICampaignCacheRepository
    {
        Task<List<Campaign>> GetCachedListAsync();

        Task<Campaign> GetByIdAsync(int campaignId);
    }
}
