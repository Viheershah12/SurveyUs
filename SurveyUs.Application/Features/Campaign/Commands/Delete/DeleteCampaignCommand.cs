using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.Campaign.Commands.Delete
{
    public class DeleteCampaignCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }

        public class DeleteCampaignCommandHandler : IRequestHandler<DeleteCampaignCommand, Result<string>> 
        {
            private readonly ICampaignRepository _campaignRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCampaignCommandHandler(ICampaignRepository campaignRepository, IUnitOfWork unitOfWork)
            {
                _campaignRepository = campaignRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<string>> Handle(DeleteCampaignCommand command, CancellationToken cancellationToken)
            {
                var campaign = await _campaignRepository.GetByIdAsync(command.Id);
                campaign.IsDeleted = true;

                await _campaignRepository.UpdateAsync(campaign);
                await _unitOfWork.Commit(cancellationToken);
                return Result<string>.Success(data: campaign.Name);
            }
        }
    }
}
