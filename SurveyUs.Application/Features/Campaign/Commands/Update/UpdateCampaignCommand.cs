using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.Campaign.Commands.Update
{
    public class UpdateCampaignCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public class UpdateCampaignCommandHandler : IRequestHandler<UpdateCampaignCommand, Result<string>>
        {
            private readonly ICampaignRepository _campaignRepository;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateCampaignCommandHandler(ICampaignRepository campaignRepository, IUnitOfWork unitOfWork)
            {
                _campaignRepository = campaignRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<string>> Handle(UpdateCampaignCommand command, CancellationToken cancellationToken)
            {
                var campaign = await _campaignRepository.GetByIdAsync(command.Id);

                if (campaign == null)
                {
                    return Result<string>.Fail("Campaign Not Found");
                }

                campaign.Name = command.Name;
                campaign.Description = command.Description;
                campaign.StartDate = command.StartDate;
                campaign.EndDate = command.EndDate;
                campaign.UpdatedOn = DateTime.Now;

                await _campaignRepository.UpdateAsync(campaign);
                await _unitOfWork.Commit(cancellationToken);
                return Result<string>.Success(data: campaign.Name);
            }
        }

    }
}
