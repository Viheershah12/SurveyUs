using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Features.UserAnswers.Queries.GetById;
using SurveyUs.Application.Interfaces.CacheRepositories;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.UserAnswers.Commands.Create
{
    public class CreateSavedAnswersCommand : IRequest<Result<bool>>
    {
        public int StoreId {  get; set; }
        public int CampaignId {  get; set; }
        public int PageNumber {  get; set; }
        public string UserId {  get; set; }
        public List<GetUserAnswersByIdResponse> Answers { get; set; } 

        public class CreateSavedAnswersCommandHandler : IRequestHandler<CreateSavedAnswersCommand, Result<bool>>
        {
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IUserAnswersCacheRepository _userAnswersCacheRepository;

            public CreateSavedAnswersCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IUserAnswersCacheRepository userAnswersCacheRepository)
            {
                _mapper = mapper;
                _userAnswersCacheRepository = userAnswersCacheRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<bool>> Handle(CreateSavedAnswersCommand query, CancellationToken cancellationToken)
            {
                try
                {
                    var answers = _mapper.Map<List<Domain.Entities.UserAnswers>>(query.Answers);

                    await _userAnswersCacheRepository.InsertSavedAnswersRangeToCache(answers, query.StoreId, query.CampaignId, query.UserId, query.PageNumber);
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
