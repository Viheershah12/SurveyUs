using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.Campaign.Commands.Create
{
    public class CreateCampaignCommand : IRequest<Result<string>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, Result<string>>
    {
        private readonly IMapper _mapper;
        private readonly ICampaignRepository _campaignRepository;

        public CreateCampaignCommandHandler(ICampaignRepository campaignRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _campaignRepository = campaignRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private IUnitOfWork _unitOfWork { get; }

        public async Task<Result<string>> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var campaign = _mapper.Map<Domain.Entities.Campaign>(request);

                campaign.CreatedOn = DateTime.Now;
                campaign.UpdatedOn = DateTime.Now;

                await _campaignRepository.InsertAsync(campaign);
                await _unitOfWork.Commit(cancellationToken);
                return Result<string>.Success(data: campaign.Name);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}