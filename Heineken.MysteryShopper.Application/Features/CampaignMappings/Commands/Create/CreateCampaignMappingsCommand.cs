using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.CampaignMappings.Commands.Create
{
    public class CreateCampaignMappingsCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int CampaignId { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class CreateCampaignMappingsCommandHandler : IRequestHandler<CreateCampaignMappingsCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly ICampaignMappingsRepository _campaignMappingsRepository;
        private IUnitOfWork _unitOfWork { get; }
        public CreateCampaignMappingsCommandHandler(IMapper mapper, ICampaignMappingsRepository campaignMappingsRepository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _campaignMappingsRepository = campaignMappingsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CreateCampaignMappingsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var campaignMapping = _mapper.Map<Domain.Entities.CampaignMappings>(request);
                await _campaignMappingsRepository.InsertAsync(campaignMapping);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(campaignMapping.StoreId);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("uq"))
                {
                    return Result<int>.Fail(request.CampaignId.ToString());
                }
                return null;
            }
        }
    }
}
