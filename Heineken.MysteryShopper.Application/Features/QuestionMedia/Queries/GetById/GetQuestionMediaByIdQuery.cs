using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;

namespace SurveyUs.Application.Features.QuestionMedia.Queries.GetById
{
    public class GetQuestionMediaByIdQuery : IRequest<Result<GetQuestionMediaByIdResponse>>
    {
        public int Id { get; set; }

        public class GetQuestionMediaByIdQueryHandler : IRequestHandler<GetQuestionMediaByIdQuery, Result<GetQuestionMediaByIdResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IQuestionMediaCacheRepository _questionMediaCacheRepository;

            public GetQuestionMediaByIdQueryHandler(
                IMapper mapper, 
                IQuestionMediaCacheRepository questionMediaCacheRepository
            )
            {
                _mapper = mapper;
                _questionMediaCacheRepository = questionMediaCacheRepository;
            }

            public async Task<Result<GetQuestionMediaByIdResponse>> Handle(GetQuestionMediaByIdQuery query,
                CancellationToken cancellationToken)
            {
                var question = await _questionMediaCacheRepository.GetCachedByIdAsync(query.Id);
                var mappedQuestion = _mapper.Map<GetQuestionMediaByIdResponse>(question);
                return Result<GetQuestionMediaByIdResponse>.Success(mappedQuestion);
            }
        }
            
    }
}
