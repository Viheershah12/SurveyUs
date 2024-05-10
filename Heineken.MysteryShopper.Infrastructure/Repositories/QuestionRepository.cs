using System;
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
    public class QuestionRepository : IQuestionRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<Question> _repository;
        public QuestionRepository(IDistributedCache distributedCache, IRepositoryAsync<Question> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<Question> Questions => _repository.Entities;

        public async Task<List<Question>> GetListAsync()
        {
            try
            {
                return await _repository.Entities.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Question> GetByIdAsync(int id)
        {
            return await _repository.Entities.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> InsertAsync(Question question)
        {
            await _repository.AddAsync(question);
            await _distributedCache.RemoveAsync(QuestionCacheKeys.ListKey);

            return question.Id;
        }

        public async Task<int> UpdateAsync(Question question)
        {
            var cacheKey = QuestionCacheKeys.GetKey(question.Id);

            await _repository.UpdateAsync(question);
            await _distributedCache.RemoveAsync(QuestionCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(cacheKey);

            return question.Id;
        }
    }
}
