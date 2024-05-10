using AutoMapper;
using SurveyUs.Application.Features.UserExtension.Commands.Create;
using SurveyUs.Application.Features.UserExtension.Commands.Delete;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Areas.Admin.Models;
using SurveyUs.Web.Areas.Users.Models;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();
            CreateMap<CreateUserExtensionCommand, BartenderViewModel>().ReverseMap();
            CreateMap<DeleteUserExtensionCommand, BartenderViewModel>().ReverseMap();
        }
    }
}