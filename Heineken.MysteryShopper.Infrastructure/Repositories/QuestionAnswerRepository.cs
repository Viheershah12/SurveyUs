using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Entities;
using SurveyUs.Infrastructure.CacheKeys;

namespace SurveyUs.Infrastructure.Repositories
{
    public class QuestionAnswerRepository : IQuestionAnswerRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<QuestionAnswers> _repository;

        public QuestionAnswerRepository(IDistributedCache distributedCache, IRepositoryAsync<QuestionAnswers> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<QuestionAnswers> QuestionAnswers => _repository.Entities;

        public IQueryable<QuestionAnswers> Questions => _repository.Entities;

        public async Task<int> InsertAsync(QuestionAnswers questionAnswers)
        {
            await _repository.AddAsync(questionAnswers);
            await _distributedCache.RemoveAsync(QuestionAnswerCacheKeys.ListKey);

            return questionAnswers.Id;
        }

        public async Task<QuestionAnswers> GetByIdAsync(int id)
        {
            return await _repository.Entities.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(QuestionAnswers questionAnswers)
        {
            await _repository.UpdateAsync(questionAnswers);
            await _distributedCache.RemoveAsync(QuestionAnswerCacheKeys.ListKey);
            return questionAnswers.Id;
        }

        public async Task<List<QuestionAnswers>> GetAllByQuestionId(int questionId)
        {
            return await _repository.Entities.Where(x => x.QuestionId == questionId).ToListAsync();
        }
    }
}
