using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface IUserExtensionRepository
    {
        IQueryable<UserExtension> UserExtension { get; }

        Task<List<UserExtension>> GetListAsync();

        Task<UserExtension> GetByIdAsync(int userExtensionId);

        Task<int> InsertAsync(UserExtension userExtension);

        Task UpdateAsync(UserExtension userExtension);

        Task DeleteAsync(UserExtension userExtension);

        Task<UserExtension> GetByEmailAsync(string email);

        Task<List<UserExtension>> GetByStoreAsync(int storeId);

        Task<UserExtension> GetByUserIdAsync(string userId);
    }
}