using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface IQuestionChoicesRepository
    {
        IQueryable<QuestionChoices> QuestionChoices { get; }

        Task<List<QuestionChoices>> GetListAsync();

        Task<QuestionChoices> GetByIdAsync(int id);

        Task<int> InsertAsync(QuestionChoices questionChoices);

        Task<int> UpdateAsync(QuestionChoices questionChoices);

        Task<bool> DeleteByQuestionIdAsync(int questionId);
    }
}
