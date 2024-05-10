using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.CampaignMappings.Commands.Update
{
    public class UpdateCampaignMappingsCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int CampaignId { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedOn { get; set; }
    }

    public class UpdateCampaignMappingsCommandHandler : IRequestHandler<UpdateCampaignMappingsCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly ICampaignMappingsRepository _campaignMappingsRepository;
        private IUnitOfWork _unitOfWork { get; }
        public UpdateCampaignMappingsCommandHandler(IMapper mapper, ICampaignMappingsRepository campaignMappingsRepository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _campaignMappingsRepository = campaignMappingsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(UpdateCampaignMappingsCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var campaignMapping = await _campaignMappingsRepository.GetByIdAsync(command.Id);

                if (campaignMapping == null)
                {
                    return Result<int>.Fail("Campaign Mapping Not Found");
                }

                campaignMapping.IsDeleted = command.IsDeleted;

                await _campaignMappingsRepository.UpdateAsync(campaignMapping);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(campaignMapping.StoreId);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail(command.CampaignId.ToString());
            }
        }
    }
}
