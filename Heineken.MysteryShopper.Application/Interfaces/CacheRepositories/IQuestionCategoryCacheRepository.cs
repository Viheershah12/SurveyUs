using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.CacheRepositories
{
    public interface IQuestionCategoryCacheRepository
    {
        Task<List<QuestionCategory>> GetCachedListAsync();

        Task<QuestionCategory> GetCachedByIdAsync(int id);
    }
}
