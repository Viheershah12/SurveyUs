using System.Security.Claims;
using AutoMapper;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class ClaimsProfile : Profile
    {
        public ClaimsProfile()
        {
            CreateMap<Claim, RoleClaimsViewModel>().ReverseMap();
        }
    }
}