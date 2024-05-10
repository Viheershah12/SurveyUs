using AutoMapper;
using SurveyUs.Application.Features.CampaignMappings.Commands.Create;
using SurveyUs.Application.Features.CampaignMappings.Queries.GetAllCachedId;
using SurveyUs.Application.Features.CampaignMappings.Queries.GetById;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    internal class CampaignMappingsProfile : Profile
    {
        public CampaignMappingsProfile() 
        {
            CreateMap<CreateCampaignMappingsCommand, CampaignMappings>().ReverseMap();
            CreateMap<GetCampaignMappingsByIdResponse, CampaignMappings>().ReverseMap();
            CreateMap<GetAllCampaignMappingsCachedResponse, CampaignMappings>().ReverseMap();
        }
    }
}
