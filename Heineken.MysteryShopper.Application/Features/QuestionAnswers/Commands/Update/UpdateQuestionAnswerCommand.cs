using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionAnswers.Commands.Update
{
    public class UpdateQuestionAnswersCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public int QuestionChoiceId { get; set; }

        public string Answer { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public class UpdateQuestionAnswersCommandHandler : IRequestHandler<UpdateQuestionAnswersCommand, Result<int>>
        {
            private readonly IMapper _mapper;
            private readonly IQuestionAnswerRepository _questionAnswerRepository;
            private IUnitOfWork _unitOfWork { get; }

            public UpdateQuestionAnswersCommandHandler(IQuestionAnswerRepository questionAnswerRepository, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _questionAnswerRepository = questionAnswerRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<Result<int>> Handle(UpdateQuestionAnswersCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var questionAnswer = await _questionAnswerRepository.GetByIdAsync(request.Id);

                    questionAnswer.IsDeleted = request.IsDeleted;
                    questionAnswer.LastModifiedBy = request.LastModifiedBy;
                    questionAnswer.LastModifiedOn = request.LastModifiedOn;

                    await _questionAnswerRepository.UpdateAsync(questionAnswer);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<int>.Success(questionAnswer.Id);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
