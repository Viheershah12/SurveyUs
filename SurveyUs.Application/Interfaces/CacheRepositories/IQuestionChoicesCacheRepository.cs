using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.CacheRepositories
{
    public interface IQuestionChoicesCacheRepository
    {
        Task<List<QuestionChoices>> GetCachedListAsync();

        Task<QuestionChoices> GetCachedByIdAsync(int id);
    }
}
