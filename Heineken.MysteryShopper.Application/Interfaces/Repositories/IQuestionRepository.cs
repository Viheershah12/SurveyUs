using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface IQuestionRepository
    {
        IQueryable<Question> Questions { get; }

        Task<List<Question>> GetListAsync();

        Task<Question> GetByIdAsync(int id);

        Task<int> InsertAsync(Question question);

        Task<int> UpdateAsync(Question question);

    }
}
