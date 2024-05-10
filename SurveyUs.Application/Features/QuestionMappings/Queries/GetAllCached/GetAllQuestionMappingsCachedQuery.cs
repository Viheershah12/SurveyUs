using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.QuestionMappings.Queries.GetAllCached
{
    public class GetAllQuestionMappingsCachedQuery : IRequest<Result<List<GetAllQuestionMappingsCachedResponse>>>
    {
    }

    public class
        GetAllQuestionMappingsCachedQueryHandler : IRequestHandler<GetAllQuestionMappingsCachedQuery,
        Result<List<GetAllQuestionMappingsCachedResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionMappingsCacheRepository _questionMappingsCache;

        public GetAllQuestionMappingsCachedQueryHandler(IQuestionMappingsCacheRepository questionMappingsCache,
            IMapper mapper)
        {
            _questionMappingsCache = questionMappingsCache;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllQuestionMappingsCachedResponse>>> Handle(
            GetAllQuestionMappingsCachedQuery request,
            CancellationToken cancellationToken)
        {
            var questionMappingsList = await _questionMappingsCache.GetCachedListAsync();
            //storeList = storeList.Where(x => !x.IsDeleted).ToList();

            var mappedQuestionMappings = _mapper.Map<List<GetAllQuestionMappingsCachedResponse>>(questionMappingsList);
            return Result<List<GetAllQuestionMappingsCachedResponse>>.Success(mappedQuestionMappings);
        }
    }
}
