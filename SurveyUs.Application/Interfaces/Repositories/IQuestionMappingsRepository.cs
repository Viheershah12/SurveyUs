using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface IQuestionMappingsRepository
    {
        IQueryable<QuestionMappings> QuestionMappings { get; }

        Task<List<QuestionMappings>> GetListAsync();

        Task<QuestionMappings> GetByIdAsync(int id);

        Task<List<QuestionMappings>> GetByCampaignIdAsync(int campaignId);

        Task DeleteAsync(QuestionMappings questionMapping);

        Task<int> InsertAsync(QuestionMappings questionMappings);

        Task UpdateAsync(QuestionMappings questionMappings);
    }
}
