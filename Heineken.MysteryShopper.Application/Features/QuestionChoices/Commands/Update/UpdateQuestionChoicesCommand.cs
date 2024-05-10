using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionChoices.Commands.Update
{
    public class UpdateQuestionChoicesCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public string ChoiceText { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; } = false;

        public class UpdateQuestionChoicesCommandHandler : IRequestHandler<UpdateQuestionChoicesCommand, Result<int>>
        {
            private readonly IMapper _mapper;
            private readonly IQuestionChoicesRepository _questionChoicesRepository;
            private IUnitOfWork _unitOfWork { get; }

            public UpdateQuestionChoicesCommandHandler(IQuestionChoicesRepository questionChoicesRepository, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _questionChoicesRepository = questionChoicesRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<Result<int>> Handle(UpdateQuestionChoicesCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var questionChoice = await _questionChoicesRepository.GetByIdAsync(request.Id);

                    questionChoice.ChoiceText = request.ChoiceText;
                    questionChoice.IsDeleted = request.IsDeleted;
                    questionChoice.LastModifiedBy = request.LastModifiedBy;
                    questionChoice.LastModifiedOn = request.LastModifiedOn;

                    await _questionChoicesRepository.UpdateAsync(questionChoice);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(questionChoice.Id);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
