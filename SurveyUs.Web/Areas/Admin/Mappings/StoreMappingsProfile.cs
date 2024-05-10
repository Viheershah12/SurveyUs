using AutoMapper;
using SurveyUs.Application.Features.StoreMappings.Commands.Create;
using SurveyUs.Application.Features.StoreMappings.Commands.Update;
using SurveyUs.Application.Features.StoreMappings.Queries.GetAllCachedId;
using SurveyUs.Application.Features.StoreMappings.Queries.GetById;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class StoreMappingsProfile : Profile
    {
        public StoreMappingsProfile()
        {
            CreateMap<GetAllStoreMappingsCachedResponse, StoreMappingsViewModel>().ReverseMap();
            CreateMap<GetStoreMappingsByIdResponse, StoreMappingsViewModel>().ReverseMap();
            CreateMap<CreateStoreMappingsCommand, StoreMappingsViewModel>().ReverseMap();
            CreateMap<UpdateStoreMappingsCommand, StoreMappingsViewModel>().ReverseMap();
        }
    }
}
