using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Enums;

namespace SurveyUs.Application.Features.Question.Commands.Create
{
    public class CreateQuestionCommand : IRequest<Result<Domain.Entities.Question>>
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public QuestionTypeEnum ResponseType { get; set; }

        public string QuestionText { get; set; }

        public int Points { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public bool HasMedia { get; set; }

        public bool HasReward { get; set; }

        public int? DisplayOrder { get; set; }
    }

    public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Result<Domain.Entities.Question>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionRepository _questionRepository;
        private IUnitOfWork _unitOfWork { get; }

        public CreateQuestionCommandHandler(IQuestionRepository questionRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Domain.Entities.Question>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var question = _mapper.Map<Domain.Entities.Question>(request);
                await _questionRepository.InsertAsync(question);
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
