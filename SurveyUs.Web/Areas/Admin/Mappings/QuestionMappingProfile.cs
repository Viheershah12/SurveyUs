using AutoMapper;
using SurveyUs.Application.Features.QuestionMappings.Commands.Create;
using SurveyUs.Application.Features.QuestionMappings.Commands.Update;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class QuestionMappingProfile : Profile
    {
        public QuestionMappingProfile()
        {
            CreateMap<CreateQuestionMappingCommand, QuestionMappings>().ReverseMap();
            CreateMap<UpdateQuestionMappingCommand, QuestionMappings>().ReverseMap();
        }
    }
}
