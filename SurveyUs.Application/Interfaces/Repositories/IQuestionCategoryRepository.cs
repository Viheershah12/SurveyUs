using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface IQuestionCategoryRepository
    {
        IQueryable<QuestionCategory> QuestionCategories { get; }

        Task<List<QuestionCategory>> GetListAsync();

        Task<QuestionCategory> GetByIdAsync(int id);
        Task<string> UpdateAsync(QuestionCategory questionCategory);
        Task<int> InsertAsync(QuestionCategory questionCategory);
    }
}
