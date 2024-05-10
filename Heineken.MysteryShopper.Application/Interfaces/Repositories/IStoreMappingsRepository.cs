using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface IStoreMappingsRepository
    {
        IQueryable<StoreMappings> StoreMappings { get; }

        Task<List<StoreMappings>> GetListAsync();

        Task<StoreMappings> GetByIdAsync(int storeId);

        Task<List<StoreMappings>> GetUsersByStoreIdAsync(int storeId);

        Task<int> InsertAsync(StoreMappings storeMapping);

        Task UpdateAsync(StoreMappings storeMapping);

        Task DeleteAsync(StoreMappings storeMapping);
    }
}
