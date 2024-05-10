using System;
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
    public class QuestionCacheRepository : IQuestionCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQuestionRepository _questionRepository;

        public QuestionCacheRepository(IDistributedCache distributedCache, IQuestionRepository campaignMappingsRepository)
        {
            _distributedCache = distributedCache;
            _questionRepository = campaignMappingsRepository;
        }

        public async Task<List<Question>> GetCachedListAsync()
        {
            var cacheKey = QuestionCacheKeys.ListKey;
            var questionList = await _distributedCache.GetAsync<List<Question>>(cacheKey);
            if (questionList == null)
            {
                questionList = await _questionRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, questionList);
            }
            return questionList;
        }
        public async Task<Question> GetCachedByIdAsync(int id)
        {
            try
            {
                var cacheKey = QuestionCacheKeys.GetKey(id);
                var question = await _distributedCache.GetAsync<Question>(cacheKey);
                if (question == null)
                {
                    question = await _questionRepository.GetByIdAsync(id);
                    Throw.Exception.IfNull(question, "Question", "No Question Found");
                    await _distributedCache.SetAsync(cacheKey, question);
                }

                return question;
            }
            catch (Exception ex)
            {
                throw new ApplicationException();
            }
        }
    }
}
