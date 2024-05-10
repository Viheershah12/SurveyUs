using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionMappings.Commands.Update
{
    public class UpdateQuestionMappingCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public int CampaignId { get; set; }

        public int QuestionId { get; set; }

        public bool IsDeleted { get; set; }

        public class UpdateQuestionMappingCommandHandler : IRequestHandler<UpdateQuestionMappingCommand, Result<int>>
        {
            private readonly IQuestionMappingsRepository _questionMappingRepository;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateQuestionMappingCommandHandler(IQuestionMappingsRepository questionMappingRepository,
                IUnitOfWork unitOfWork)
            {
                _questionMappingRepository = questionMappingRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateQuestionMappingCommand command,
                CancellationToken cancellationToken)
            {
                var questionMapping = await _questionMappingRepository.GetByIdAsync(command.Id);

                if (questionMapping == null)
                {
                    return Result<int>.Fail("PhysicalUserAnswers Not Found.");
                }

                await _questionMappingRepository.UpdateAsync(questionMapping);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(questionMapping.Id);
            }
        }
    }
}
