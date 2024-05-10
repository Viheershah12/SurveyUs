using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.Question.Commands.Update
{
    public class UpdateQuestionCommand : IRequest<Result<Domain.Entities.Question>>
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        //public QuestionTypeEnum ResponseType { get; set; }
        public string QuestionText { get; set; }
        public int Points { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool HasMedia { get; set; }
        public bool HasReward { get; set; }
        public int? DisplayOrder { get; set; }

        public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, Result<Domain.Entities.Question>> 
        {
            private readonly IQuestionRepository _questionRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public UpdateQuestionCommandHandler(IQuestionRepository questionRepository, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _questionRepository = questionRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<Result<Domain.Entities.Question>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var question = await _questionRepository.GetByIdAsync(request.Id);

                    if (question == null)
                    {
                        return Result<Domain.Entities.Question>.Fail("Question Not Found.");
                    }

                    question.QuestionText = request.QuestionText;
                    question.CategoryId = request.CategoryId;
                    question.Points = request.Points;
                    question.UpdatedOn = request.UpdatedOn;
                    question.UpdatedBy = request.UpdatedBy;
                    question.HasMedia = request.HasMedia;
                    question.HasReward = request.HasReward;
                    question.DisplayOrder = request.DisplayOrder;

                    await _questionRepository.UpdateAsync(question);
                    await _unitOfWork.Commit(cancellationToken);
                    return Result<Domain.Entities.Question>.Success(question);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
