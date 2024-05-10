using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.CacheRepositories
{
    public interface IUserExtensionCacheRepository
    {
        Task<List<UserExtension>> GetCachedListAsync();

        Task<UserExtension> GetByIdAsync(int userExtensionId);
    }
}