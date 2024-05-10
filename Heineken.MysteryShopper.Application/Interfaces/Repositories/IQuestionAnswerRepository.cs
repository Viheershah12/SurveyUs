using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface IQuestionAnswerRepository
    {
        IQueryable<QuestionAnswers> QuestionAnswers { get; }

        Task<int> InsertAsync(QuestionAnswers questionAnswers);

        Task<QuestionAnswers> GetByIdAsync(int id);

        Task<int> UpdateAsync(QuestionAnswers questionAnswers);

        Task<List<QuestionAnswers>> GetAllByQuestionId(int questionId);
    }
}
