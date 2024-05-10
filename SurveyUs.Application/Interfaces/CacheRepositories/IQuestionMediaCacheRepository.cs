using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.CacheRepositories
{
    public interface IQuestionMediaCacheRepository
    {
        Task<List<QuestionMedia>> GetCachedListAsync();

        Task<QuestionMedia> GetCachedByIdAsync(int id);

        Task<bool> SaveQuestionFileToCache(List<QuestionMedia> questionMedia, int storeId, int campaignId,
            string userId, int pageNumber);

        Task<List<QuestionMedia>> GetSavedQuestionFileByPageNumber(int storeId, int campaignId, string userId,
            int pageNumber);
    }
}
