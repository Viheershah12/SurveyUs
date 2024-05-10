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
    public class QuestionCategoryRepository : IQuestionCategoryRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<QuestionCategory> _repository;
        public QuestionCategoryRepository(IDistributedCache distributedCache, IRepositoryAsync<QuestionCategory> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<QuestionCategory> QuestionCategories => _repository.Entities;

        public async Task<List<QuestionCategory>> GetListAsync()
        {
            try
            {
                return await _repository.Entities.Where(x => !x.IsDeleted).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<QuestionCategory> GetByIdAsync(int id)
        {
            return await _repository.Entities.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> InsertAsync(QuestionCategory questionCategory)
        {
            await _repository.AddAsync(questionCategory);
            await _distributedCache.RemoveAsync(QuestionCategoryCacheKeys.ListKey);
            return questionCategory.Id;
        }

        public async Task<string> UpdateAsync(QuestionCategory questionCategory)
        {
            await _repository.UpdateAsync(questionCategory);
            await _distributedCache.RemoveAsync(QuestionCategoryCacheKeys.ListKey);
            return questionCategory.CategoryName;
        }
    }
}
