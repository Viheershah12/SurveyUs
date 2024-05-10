using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionMedia.Queries.GetById
{
    public class GetSavedFileMediaByPageNumberQuery : IRequest<Result<List<GetQuestionMediaByIdResponse>>>
    {
        public int StoreId { get; set; }
        public int CampaignId { get; set; }
        public int PageNumber { get; set; }
        public string UserId { get; set; }

        public class GetSavedFileMediaByPageNumberQueryHandler : IRequestHandler<GetSavedFileMediaByPageNumberQuery, Result<List<GetQuestionMediaByIdResponse>>>
        {
            private readonly IMapper _mapper;
            private readonly IQuestionMediaCacheRepository _quesstionMediaCacheRepository;
            private readonly IUnitOfWork _unitOfWork;

            public GetSavedFileMediaByPageNumberQueryHandler(IMapper mapper, IQuestionMediaCacheRepository questionMediaCacheRepository, IUnitOfWork unitOfWork)
            {
                _mapper = mapper;
                _unitOfWork = unitOfWork;
                _quesstionMediaCacheRepository = questionMediaCacheRepository;
            }

            public async Task<Result<List<GetQuestionMediaByIdResponse>>> Handle(GetSavedFileMediaByPageNumberQuery request, CancellationToken cancellationToken)
            {
                var media = await _quesstionMediaCacheRepository.GetSavedQuestionFileByPageNumber(request.StoreId, request.CampaignId, request.UserId, request.PageNumber);
                var mappedMedia = _mapper.Map<List<GetQuestionMediaByIdResponse>>(media);

                return Result<List<GetQuestionMediaByIdResponse>>.Success(mappedMedia);
            }
        }
    }
}
