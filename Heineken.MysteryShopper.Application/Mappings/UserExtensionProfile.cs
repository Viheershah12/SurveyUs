using AutoMapper;
using SurveyUs.Application.Features.UserExtension.Commands.Create;
using SurveyUs.Application.Features.UserExtension.Commands.Update;
using SurveyUs.Application.Features.UserExtension.Queries.GetAllCached;
using SurveyUs.Application.Features.UserExtension.Queries.GetById;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    internal class UserExtensionProfile : Profile
    {
        public UserExtensionProfile()
        {
            CreateMap<CreateUserExtensionCommand, UserExtension>().ReverseMap();
            CreateMap<UpdateUserExtensionCommand, UserExtension>().ReverseMap();
            CreateMap<GetUserExtensionByIdResponse, UserExtension>().ReverseMap();
            CreateMap<GetAllUserExtensionCachedResponse, UserExtension>().ReverseMap();
        }
    }
}