using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<IdentityRole, RoleViewModel>().ReverseMap();
        }
    }
}