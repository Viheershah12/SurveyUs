using AutoMapper;
using SurveyUs.Application.Features.CampaignMappings.Commands.Update;
using SurveyUs.Domain.Entities;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        { 
            CreateMap<CampaignMappings, CampaignSettingViewModel>().ReverseMap();
            CreateMap<CampaignMappings, UpdateCampaignMappingsCommand>().ReverseMap();
        }
    }
}
