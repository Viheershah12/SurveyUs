using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionAnswers.Commands.Create
{
    public class CreateQuestionAnswerCommand : IRequest<Result<Domain.Entities.QuestionAnswers>>
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
    }

    public class CreateQuestionAnswerCommandHandler : IRequestHandler<CreateQuestionAnswerCommand, Result<Domain.Entities.QuestionAnswers>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionAnswerRepository _questionAnswerRepository;
        private IUnitOfWork _unitOfWork { get; }

        public CreateQuestionAnswerCommandHandler(IQuestionAnswerRepository questionAnswerRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _questionAnswerRepository = questionAnswerRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Domain.Entities.QuestionAnswers>> Handle(CreateQuestionAnswerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var question = _mapper.Map<Domain.Entities.QuestionAnswers>(request);
                await _questionAnswerRepository.InsertAsync(question);
                await _unitOfWork.Commit(cancellationToken);

                return Result<Domain.Entities.QuestionAnswers>.Success(question);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
