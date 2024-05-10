using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SurveyUs.Application.Interfaces.CacheRepositories;
using SurveyUs.Application.Interfaces.Contexts;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Infrastructure.CacheRepositories;
using SurveyUs.Infrastructure.DbContexts;
using SurveyUs.Infrastructure.Repositories;

namespace SurveyUs.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistenceContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            #region Repositories

            services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
            services.AddTransient<IStoreRepository, StoreRepository>();
            services.AddTransient<IStoreCacheRepository, StoreCacheRepository>();
            services.AddTransient<IStoreMappingsRepository, StoreMappingsRepository>();
            services.AddTransient<IStoreMappingsCacheRepository, StoreMappingsCacheRepository>();
            services.AddTransient<ICampaignRepository, CampaignRepository>();
            services.AddTransient<ICampaignCacheRepository, CampaignCacheRepository>();
            services.AddTransient<ICampaignMappingsRepository, CampaignMappingsRepository>();
            services.AddTransient<ICampaignMappingsCacheRepository, CampaignMappingsCacheRepository>();
            services.AddTransient<IUserExtensionRepository, UserExtensionRepository>();
            services.AddTransient<IUserExtensionCacheRepository, UserExtensionCacheRepository>();
            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IQuestionCacheRepository, QuestionCacheRepository>();
            services.AddTransient<IQuestionRepository, QuestionRepository>();
            services.AddTransient<IQuestionChoicesCacheRepository, QuestionChoicesCacheRepository>();
            services.AddTransient<IQuestionChoicesRepository, QuestionChoicesRepository>();
            services.AddTransient<IQuestionMappingsCacheRepository, QuestionMappingsCacheRepository>();
            services.AddTransient<IQuestionMappingsRepository, QuestionMappingsRepository>();
            services.AddTransient<IUserAnswersCacheRepository, UserAnswersCacheRepository>();
            services.AddTransient<IUserAnswersRepository, UserAnswersRepository>();
            services.AddTransient<IQuestionCategoryCacheRepository, QuestionCategoryCacheRepository>();
            services.AddTransient<IQuestionCategoryRepository, QuestionCategoryRepository>();
            services.AddTransient<IQuestionCategoryMappingRepository, QuestionCategoryMappingRepository>();
            services.AddTransient<IQuestionCategoryMappingCacheRepository, QuestionCategoryMappingCacheRepository>();
            services.AddTransient<IQuestionMediaRepository, QuestionMediaRepository>();
            services.AddTransient<IQuestionMediaCacheRepository, QuestionMediaCacheRepository>();
            services.AddTransient<IQuestionAnswerRepository, QuestionAnswerRepository>();
            services.AddTransient<IQuestionAnswerCacheRepository, QuestionAnswerCacheRepository>();

            #endregion Repositories
        }
    }
}