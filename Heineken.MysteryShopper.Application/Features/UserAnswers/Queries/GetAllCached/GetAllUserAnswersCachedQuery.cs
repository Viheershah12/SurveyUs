using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.UserAnswers.Queries.GetAllCached
{
    public class GetAllUserAnswersCachedQuery : IRequest<Result<List<GetAllUserAnswersCachedResponse>>>
    {
    }

    public class
        GetAllUserAnswersCachedQueryHandler : IRequestHandler<GetAllUserAnswersCachedQuery,
        Result<List<GetAllUserAnswersCachedResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IUserAnswersCacheRepository _userAnswersCacheRepository;

        public GetAllUserAnswersCachedQueryHandler(IUserAnswersCacheRepository userAnswersCacheRepository,
            IMapper mapper)
        {
            _userAnswersCacheRepository = userAnswersCacheRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllUserAnswersCachedResponse>>> Handle(
            GetAllUserAnswersCachedQuery request,
            CancellationToken cancellationToken)
        {
            var userAnswersList = await _userAnswersCacheRepository.GetCachedListAsync();
            //storeList = storeList.Where(x => !x.IsDeleted).ToList();

            var mappedUserAnswers = _mapper.Map<List<GetAllUserAnswersCachedResponse>>(userAnswersList);
            return Result<List<GetAllUserAnswersCachedResponse>>.Success(mappedUserAnswers);
        }
    }
}
