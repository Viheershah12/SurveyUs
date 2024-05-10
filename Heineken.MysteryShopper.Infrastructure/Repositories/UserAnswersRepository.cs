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
    public class UserAnswersRepository : IUserAnswersRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<UserAnswers> _repository;

        public UserAnswersRepository(IDistributedCache distributedCache, IRepositoryAsync<UserAnswers> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<UserAnswers> UserAnswers => _repository.Entities;

        public async Task<List<UserAnswers>> GetListAsync()
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

        public async Task<UserAnswers> GetByIdAsync(int id)
        {
            return await _repository.Entities.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> InsertAsync(UserAnswers userAnswers)
        {
            await _repository.AddAsync(userAnswers);
            await _distributedCache.RemoveAsync(UserAnswersCacheKeys.ListKey);
            return userAnswers.Id;
        }
    }
}
