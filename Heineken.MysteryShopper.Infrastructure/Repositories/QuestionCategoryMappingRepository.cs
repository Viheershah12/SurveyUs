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
    public class QuestionCategoryMappingRepository : IQuestionCategoryMappingRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<QuestionCategoryMapping> _repository;

        public QuestionCategoryMappingRepository(IDistributedCache distributedCache, IRepositoryAsync<QuestionCategoryMapping> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<QuestionCategoryMapping> QuestionCategoryMappings => _repository.Entities;

        public async Task<List<QuestionCategoryMapping>> GetListAsync()
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

        public async Task<QuestionCategoryMapping> GetByIdAsync(int id)
        {
            return await _repository.Entities.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> InsertAsync(QuestionCategoryMapping questionCategory)
        {
            await _repository.AddAsync(questionCategory);
            await _distributedCache.RemoveAsync(QuestionCategoryMappingCacheKeys.ListKey);
            return questionCategory.Id;
        }
    }
}
