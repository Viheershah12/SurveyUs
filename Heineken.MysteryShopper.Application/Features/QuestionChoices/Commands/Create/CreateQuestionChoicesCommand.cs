using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionChoices.Commands.Create
{
    public class CreateQuestionChoicesCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public string ChoiceText { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class CreateQuestionChoicesCommandHandler : IRequestHandler<CreateQuestionChoicesCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionChoicesRepository _questionChoicesRepository;
        private IUnitOfWork _unitOfWork { get; }

        public CreateQuestionChoicesCommandHandler(IQuestionChoicesRepository questionChoicesRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _questionChoicesRepository = questionChoicesRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateQuestionChoicesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var questionChoices = _mapper.Map<Domain.Entities.QuestionChoices>(request);

                await _questionChoicesRepository.InsertAsync(questionChoices);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(questionChoices.Id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
