using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.CacheRepositories
{
    public interface IUserAnswersCacheRepository
    {
        Task<List<UserAnswers>> GetCachedListAsync();

        Task<UserAnswers> GetCachedByIdAsync(int id);

        Task<bool> InsertSavedAnswersRangeToCache(List<UserAnswers> answers, int storeId, int campaignId, string userId, int pageNumber);

        Task<List<UserAnswers>> GetSavedAnswersByPageNumber(int storeId, int campaignId, string userId, int pageNumber);

        Task<bool> DeleteSavedAnswers(int storeId, int campaignId, string userId, int totalPage);
    }
}
