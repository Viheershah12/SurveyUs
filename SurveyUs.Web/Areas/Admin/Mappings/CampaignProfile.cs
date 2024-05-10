using AutoMapper;
using SurveyUs.Application.Features.Campaign.Commands.Create;
using SurveyUs.Application.Features.Campaign.Commands.Update;
using SurveyUs.Application.Features.Campaign.Queries.GetAllCached;
using SurveyUs.Application.Features.Campaign.Queries.GetById;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class CampaignProfile : Profile
    {
        public CampaignProfile() 
        {
            CreateMap<CampaignSettingViewModel, CreateCampaignCommand>().ReverseMap();
            CreateMap<CampaignSettingViewModel, GetAllCampaignsCachedResponse>().ReverseMap();
            CreateMap<CampaignSettingViewModel, GetCampaignByIdResponse>().ReverseMap();
            CreateMap<CampaignSettingViewModel, UpdateCampaignCommand>().ReverseMap();
            CreateMap<CampaignSettingViewModel, Domain.Entities.Campaign>().ReverseMap();
            CreateMap<UpdateCampaignCommand, GetCampaignByIdResponse>().ReverseMap();
        }
    }
}
