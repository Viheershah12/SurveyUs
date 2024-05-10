using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.CacheRepositories
{
    public interface IQuestionCacheRepository
    {
        Task<List<Question>> GetCachedListAsync();

        Task<Question> GetCachedByIdAsync(int id);
    }
}
