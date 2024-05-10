using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionMappings.Commands.Create
{
    public class CreateQuestionMappingCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public int CampaignId { get; set; }

        public int QuestionId { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    public class CreateQuestionMappingCommandHandler : IRequestHandler<CreateQuestionMappingCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionMappingsRepository _questionMappingRepository;
        private IUnitOfWork _unitOfWork { get; }

        public CreateQuestionMappingCommandHandler(IQuestionMappingsRepository questionMappingRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _questionMappingRepository = questionMappingRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateQuestionMappingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var question = _mapper.Map<Domain.Entities.QuestionMappings>(request);

                await _questionMappingRepository.InsertAsync(question);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(question.Id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
