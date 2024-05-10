using AutoMapper;
using SurveyUs.Application.Features.UserExtension.Commands.Create;
using SurveyUs.Application.Features.UserExtension.Commands.Delete;
using SurveyUs.Infrastructure.Identity.Models;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class UserExtensionProfile : Profile
    {
        public UserExtensionProfile()
        {
            CreateMap<ApplicationUser, UserExtensionViewModel>().ReverseMap();
            CreateMap<CreateUserExtensionCommand, UserExtensionViewModel>().ReverseMap();
            CreateMap<DeleteUserExtensionCommand, UserExtensionViewModel>().ReverseMap();
        }
    }
}