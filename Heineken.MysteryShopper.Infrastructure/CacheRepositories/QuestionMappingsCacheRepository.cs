using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using Microsoft.Extensions.Caching.Distributed;
using SurveyUs.Application.Interfaces.CacheRepositories;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Entities;
using SurveyUs.Infrastructure.CacheKeys;

namespace SurveyUs.Infrastructure.CacheRepositories
{
    public class QuestionMappingsCacheRepository : IQuestionMappingsCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQuestionMappingsRepository _questionMappingsRepository;

        public QuestionMappingsCacheRepository(IDistributedCache distributedCache, IQuestionMappingsRepository questionMappingsRepository)
        {
            _distributedCache = distributedCache;
            _questionMappingsRepository = questionMappingsRepository;
        }

        public async Task<List<QuestionMappings>> GetCachedListAsync()
        {
            var cacheKey = QuestionMappingsCacheKeys.ListKey;
            var questionMappingsList = await _distributedCache.GetAsync<List<QuestionMappings>>(cacheKey);
            if (questionMappingsList == null)
            {
                questionMappingsList = await _questionMappingsRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, questionMappingsList);
            }
            return questionMappingsList;
        }

        public async Task<QuestionMappings> GetCachedByIdAsync(int id)
        {
            var cacheKey = QuestionMappingsCacheKeys.GetKey(id);
            var questionMappings = await _distributedCache.GetAsync<QuestionMappings>(cacheKey);
            if (questionMappings == null)
            {
                questionMappings = await _questionMappingsRepository.GetByIdAsync(id);
                Throw.Exception.IfNull(questionMappings, "questionMappings", "No QuestionMappings Found");
                await _distributedCache.SetAsync(cacheKey, questionMappings);
            }

            return questionMappings;
        }
    }
}
