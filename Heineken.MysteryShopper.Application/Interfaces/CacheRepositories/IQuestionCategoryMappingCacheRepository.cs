using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.CacheRepositories
{
    public interface IQuestionCategoryMappingCacheRepository
    {
        Task<List<QuestionCategoryMapping>> GetCachedListAsync();

        Task<QuestionCategoryMapping> GetCachedByIdAsync(int id);
    }
}
