using AutoMapper;
using SurveyUs.Application.Features.UserExtension.Commands.Create;
using SurveyUs.Application.Features.UserExtension.Queries.GetAllCached;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Areas.Users.Models;

namespace SurveyUs.Web.Areas.Users.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UsersViewModel>().ReverseMap();
            CreateMap<GetAllUserExtensionCachedResponse, BartenderViewModel>().ReverseMap();
            CreateMap<CreateUserExtensionCommand, BartenderViewModel>().ReverseMap();
        }
    }
}
