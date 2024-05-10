using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface IQuestionCategoryMappingRepository
    {
        IQueryable<QuestionCategoryMapping> QuestionCategoryMappings { get; }

        Task<List<QuestionCategoryMapping>> GetListAsync();

        Task<QuestionCategoryMapping> GetByIdAsync(int id);

        Task<int> InsertAsync(QuestionCategoryMapping questionCategoryMapping);
    }
}
