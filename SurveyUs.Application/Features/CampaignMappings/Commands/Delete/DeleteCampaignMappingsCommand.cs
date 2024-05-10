using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.CampaignMappings.Commands.Delete
{
    public class DeleteCampaignMappingsCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteCampaignMappingsCommandHandler : IRequestHandler<DeleteCampaignMappingsCommand, Result<int>>
        {
            private readonly ICampaignMappingsRepository _campaignMappingsRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCampaignMappingsCommandHandler(ICampaignMappingsRepository campaignMappingsRepository, IUnitOfWork unitOfWork)
            {
                _campaignMappingsRepository = campaignMappingsRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteCampaignMappingsCommand command, CancellationToken cancellationToken)
            {
                var campaignMapping = await _campaignMappingsRepository.GetByIdAsync(command.Id);
                //await _campaignMappingsRepository.UpdateAsync(campaignMapping);
                //campaignMapping.IsDeleted = true;
                await _campaignMappingsRepository.DeleteAsync(campaignMapping);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(campaignMapping.Id);
            }
        }
    }
}
