using AutoMapper;
using SurveyUs.Application.Features.UserExtension.Commands.Create;
using SurveyUs.Application.Features.UserExtension.Commands.Update;
using SurveyUs.Application.Features.UserExtension.Queries.GetAllCached;
using SurveyUs.Application.Features.UserExtension.Queries.GetById;
using SurveyUs.Web.Areas.Users.Models;

namespace SurveyUs.Web.Areas.Users.Mappings
{
    internal class UserExtensionProfile : Profile
    {
        public UserExtensionProfile()
        {
            CreateMap<GetAllUserExtensionCachedResponse, BartenderViewModel>().ReverseMap();
            CreateMap<GetUserExtensionByIdResponse, BartenderViewModel>().ReverseMap();
            CreateMap<CreateUserExtensionCommand, BartenderViewModel>().ReverseMap();
            CreateMap<UpdateUserExtensionCommand, BartenderViewModel>().ReverseMap();
        }
    }
}