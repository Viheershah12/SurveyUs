using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.CacheRepositories;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.UserAnswers.Commands.Create
{
    public class CreateUserAnswersCommand : IRequest<Result<bool>>
    {
        public List<Domain.Entities.UserAnswers> Answers { get; set; }
    }

    public class CreateUserAnswersCommandHandler : IRequestHandler<CreateUserAnswersCommand, Result<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUserAnswersRepository _userAnswersRepository;
        private readonly IUserAnswersCacheRepository _userAnswersCacheRepository;
        private IUnitOfWork _unitOfWork { get; }

        public CreateUserAnswersCommandHandler(IUserAnswersRepository userAnswersRepository, IUserAnswersCacheRepository userAnswersCacheRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userAnswersRepository = userAnswersRepository;
            _userAnswersCacheRepository = userAnswersCacheRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<bool>> Handle(CreateUserAnswersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Answers.Count > 0)
                {
                    foreach (var answer in request.Answers)
                    {
                        answer.CreatedOn = DateTime.Now;
                        answer.UpdatedOn = DateTime.Now;

                        await _userAnswersRepository.InsertAsync(answer);
                    }

                    int storeId = request.Answers[0].StoreId;
                    int campaignId = request.Answers[0].CampaignId;
                    int totalPage = Convert.ToInt32(Math.Ceiling(request.Answers.Count / 5.0));
                    string userId = request.Answers[0].UserId;

                    await _unitOfWork.Commit(cancellationToken);

                    var _cacheKey = _userAnswersCacheRepository.DeleteSavedAnswers(storeId, campaignId, userId, totalPage);
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Success(false);
            }
            catch (Exception ex)
            {
                await _unitOfWork.Rollback();
                return Result<bool>.Success(false);
            }
        }
    }
}
