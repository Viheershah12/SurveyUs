using AutoMapper;
using SurveyUs.Application.Features.Store.Commands.Create;
using SurveyUs.Application.Features.Store.Queries.GetAllCached;
using SurveyUs.Application.Features.Store.Queries.GetById;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    internal class StoreProfile : Profile
    {
        public StoreProfile()
        {
            CreateMap<CreateStoreCommand, Store>().ReverseMap();
            CreateMap<GetStoreByIdResponse, Store>().ReverseMap();
            CreateMap<GetAllStoresCachedResponse, Store>().ReverseMap();
        }
    }
}