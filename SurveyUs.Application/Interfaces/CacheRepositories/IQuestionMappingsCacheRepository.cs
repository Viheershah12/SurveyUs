using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.CacheRepositories
{
    public interface IQuestionMappingsCacheRepository
    {
        Task<List<QuestionMappings>> GetCachedListAsync();

        Task<QuestionMappings> GetCachedByIdAsync(int id);
    }
}
