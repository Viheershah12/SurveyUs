using AutoMapper;
using SurveyUs.Application.Features.StoreMappings.Commands.Create;
using SurveyUs.Application.Features.StoreMappings.Queries.GetAllCachedId;
using SurveyUs.Application.Features.StoreMappings.Queries.GetById;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    public class StoreMappingsProfile : Profile
    {
        public StoreMappingsProfile()
        {
            CreateMap<CreateStoreMappingsCommand, StoreMappings>().ReverseMap();
            CreateMap<GetStoreMappingsByIdResponse, StoreMappings>().ReverseMap();
            CreateMap<GetAllStoreMappingsCachedResponse, StoreMappings>().ReverseMap();
        }
    }
}
