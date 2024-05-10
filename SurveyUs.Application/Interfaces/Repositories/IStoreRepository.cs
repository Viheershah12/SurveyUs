using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface IStoreRepository
    {
        IQueryable<Store> Store { get; }

        Task<List<Store>> GetListAsync();

        Task<Store> GetByIdAsync(int storeId);

        Task<int> InsertAsync(Store store);

        Task UpdateAsync(Store store);

        Task DeleteAsync(Store store);

        Task<Store> GetByStateAsync(int state);
    }
}