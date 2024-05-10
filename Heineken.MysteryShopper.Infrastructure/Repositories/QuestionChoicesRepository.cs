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
    public class QuestionChoicesRepository : IQuestionChoicesRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<QuestionChoices> _repository;
        public QuestionChoicesRepository(IDistributedCache distributedCache, IRepositoryAsync<QuestionChoices> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<QuestionChoices> QuestionChoices => _repository.Entities;

        public async Task<List<QuestionChoices>> GetListAsync()
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

        public async Task<QuestionChoices> GetByIdAsync(int id)
        {
            return await _repository.Entities.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> InsertAsync(QuestionChoices questionChoices)
        {
            await _repository.AddAsync(questionChoices);
            await _distributedCache.RemoveAsync(QuestionChoicesCacheKeys.ListKey);
            return questionChoices.Id;
        }

        public async Task<int> UpdateAsync(QuestionChoices questionChoices)
        {
            await _repository.UpdateAsync(questionChoices);
            await _distributedCache.RemoveAsync(QuestionChoicesCacheKeys.ListKey);
            return questionChoices.Id;
        }

        public async Task<bool> DeleteByQuestionIdAsync(int questionId)
        {
            var choices = _repository.Entities.Where(c => c.QuestionId == questionId).ToList();

            foreach (var questionChoices in choices)
            {
                await _repository.DeleteAsync(questionChoices);
                await _distributedCache.RemoveAsync(QuestionChoicesCacheKeys.GetKey(questionChoices.Id));
            }
            await _distributedCache.RemoveAsync(QuestionChoicesCacheKeys.ListKey);

            return true;
        }
    }
}
