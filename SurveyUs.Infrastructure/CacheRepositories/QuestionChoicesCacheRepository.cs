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
    public class QuestionChoicesCacheRepository : IQuestionChoicesCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQuestionChoicesRepository _questionChoicesRepository;

        public QuestionChoicesCacheRepository(IDistributedCache distributedCache, IQuestionChoicesRepository questionChoicesRepository)
        {
            _distributedCache = distributedCache;
            _questionChoicesRepository = questionChoicesRepository;
        }

        public async Task<List<QuestionChoices>> GetCachedListAsync()
        {
            var cacheKey = QuestionChoicesCacheKeys.ListKey;
            var questionChoicesList = await _distributedCache.GetAsync<List<QuestionChoices>>(cacheKey);
            if (questionChoicesList == null)
            {
                questionChoicesList = await _questionChoicesRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, questionChoicesList);
            }
            return questionChoicesList;
        }

        public async Task<QuestionChoices> GetCachedByIdAsync(int id)
        {
            var cacheKey = QuestionChoicesCacheKeys.GetKey(id);
            var questionChoices = await _distributedCache.GetAsync<QuestionChoices>(cacheKey);
            if (questionChoices == null)
            {
                questionChoices = await _questionChoicesRepository.GetByIdAsync(id);
                Throw.Exception.IfNull(questionChoices, "QuestionChoices", "No QuestionChoice Found");
                await _distributedCache.SetAsync(cacheKey, questionChoices);
            }

            return questionChoices;
        }
    }
}
