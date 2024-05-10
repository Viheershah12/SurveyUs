using AutoMapper;
using SurveyUs.Application.Features.CampaignMappings.Commands.Create;
using SurveyUs.Application.Features.CampaignMappings.Commands.Update;
using SurveyUs.Application.Features.CampaignMappings.Queries.GetAllCachedId;
using SurveyUs.Application.Features.CampaignMappings.Queries.GetById;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class CampaignMappingsProfile : Profile
    {
        public CampaignMappingsProfile() 
        {
            CreateMap<GetAllCampaignMappingsCachedResponse, CampaignMappingsViewModel>().ReverseMap();
            CreateMap<GetCampaignMappingsByIdResponse, CampaignMappingsViewModel>().ReverseMap();
            CreateMap<CreateCampaignMappingsCommand, CampaignMappingsViewModel>().ReverseMap();
            CreateMap<UpdateCampaignMappingsCommand, CampaignMappingsViewModel>().ReverseMap();
        }
    }
}
