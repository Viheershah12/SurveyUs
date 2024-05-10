using AutoMapper;
using SurveyUs.Application.Features.QuestionCategoryMapping.Commands.Create;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class QuestionCategoryMappingProfile : Profile
    {
        public QuestionCategoryMappingProfile()
        {
            CreateMap<CreateQuestionCategoryMappingCommand, QuestionCategoryMapping>().ReverseMap();
        }
    }
}
