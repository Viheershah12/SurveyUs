using AutoMapper;
using SurveyUs.Application.Features.Campaign.Commands.Create;
using SurveyUs.Application.Features.Campaign.Queries.GetAllCached;
using SurveyUs.Application.Features.Campaign.Queries.GetById;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    internal class CampaignProfile : Profile
    {
        public CampaignProfile() 
        {
            CreateMap<GetAllCampaignsCachedResponse, Campaign>().ReverseMap();
            CreateMap<CreateCampaignCommand, Campaign>().ReverseMap();
            CreateMap<GetCampaignByIdResponse, Campaign>().ReverseMap();
        }
    }
}
