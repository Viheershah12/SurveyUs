using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionMedia.Commands.Create
{
    public class CreateSavedMediaCommand : IRequest<Result<bool>>
    {
        public int StoreId { get; set; }
        public int CampaignId { get; set; }
        public int PageNumber { get; set; }
        public string UserId { get; set; }
        public List<CreateQuestionMediaCommand> QuestionMedia { get; set; }

        public class CreateSavedMediaCommandHandler : IRequestHandler<CreateSavedMediaCommand, Result<bool>>
        {
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IQuestionMediaCacheRepository _questionMediaCacheRepository;

            public CreateSavedMediaCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IQuestionMediaCacheRepository questionMediaCacheRepository)
            {
                _mapper = mapper;
                _unitOfWork = unitOfWork;
                _questionMediaCacheRepository = questionMediaCacheRepository;
            }

            public async Task<Result<bool>> Handle(CreateSavedMediaCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var questionMedia = _mapper.Map<List<Domain.Entities.QuestionMedia>>(request.QuestionMedia);

                    await _questionMediaCacheRepository.SaveQuestionFileToCache(questionMedia, request.StoreId, request.CampaignId, request.UserId, request.PageNumber);
                    await _unitOfWork.Commit(cancellationToken);

                    return Result<bool>.Success("Cached Successfully");
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
