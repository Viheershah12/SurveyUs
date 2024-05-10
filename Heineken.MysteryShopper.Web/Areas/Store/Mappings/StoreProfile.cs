using AutoMapper;
using SurveyUs.Application.Features.Store.Commands.Create;
using SurveyUs.Application.Features.Store.Commands.Update;
using SurveyUs.Application.Features.Store.Queries.GetAllCached;
using SurveyUs.Application.Features.Store.Queries.GetById;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Store.Mappings
{
    public class StoreProfile : Profile
    {
        public StoreProfile()
        {
            CreateMap<GetAllStoresCachedResponse, StoreViewModel>().ReverseMap();
            CreateMap<GetStoreByIdResponse, StoreViewModel>().ReverseMap();
            CreateMap<CreateStoreCommand, StoreViewModel>().ReverseMap();
            CreateMap<UpdateStoreCommand, StoreViewModel>().ReverseMap();
            CreateMap<Domain.Entities.Store, StoreViewModel>().ReverseMap();
        }
    }
}
