using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface IUserAnswersRepository
    {
        IQueryable<UserAnswers> UserAnswers { get;  }

        Task<List<UserAnswers>> GetListAsync();

        Task<UserAnswers> GetByIdAsync(int id);

        Task<int> InsertAsync(UserAnswers userAnswers);
    }
}
