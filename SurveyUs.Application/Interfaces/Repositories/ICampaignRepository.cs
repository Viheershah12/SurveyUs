using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface ICampaignRepository
    {
        IQueryable<Campaign> Campaign { get; }

        Task<List<Campaign>> GetListAsync();

        Task<Campaign> GetByIdAsync(int storeId);

        Task<int> InsertAsync(Campaign campaign);

        Task UpdateAsync(Campaign campaign);

        Task DeleteAsync(Campaign campaign);
    }
}
