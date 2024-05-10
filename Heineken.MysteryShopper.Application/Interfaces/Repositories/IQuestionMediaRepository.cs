using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface IQuestionMediaRepository
    {
        IQueryable<QuestionMedia> QuestionMedia { get; }

        Task<List<QuestionMedia>> GetListAsync();

        Task<QuestionMedia> GetByIdAsync(int id);

        Task<int> InsertAsync(QuestionMedia questionMedia);

        Task<int> UpdateAsync(QuestionMedia questionMedia);

        Task<List<QuestionMedia>> GetByQuestionId(int questionId, int campaignId, int storeId, string userId);
    }
}
