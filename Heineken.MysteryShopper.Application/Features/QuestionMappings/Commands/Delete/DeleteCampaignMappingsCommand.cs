using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionMappings.Commands.Delete
{
    public class DeleteQuestionMappingsCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteCampaignMappingsCommandHandler : IRequestHandler<DeleteQuestionMappingsCommand, Result<int>>
        {
            private readonly IQuestionMappingsRepository _questionMappingsRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCampaignMappingsCommandHandler(IQuestionMappingsRepository questionMappingsRepository, IUnitOfWork unitOfWork)
            {
                _questionMappingsRepository = questionMappingsRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteQuestionMappingsCommand command, CancellationToken cancellationToken)
            {
                var campaignMapping = await _questionMappingsRepository.GetByIdAsync(command.Id);
                await _questionMappingsRepository.DeleteAsync(campaignMapping);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(campaignMapping.Id);
            }
        }
    }
}
