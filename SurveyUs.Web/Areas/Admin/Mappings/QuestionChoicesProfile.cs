using AutoMapper;
using SurveyUs.Application.Features.QuestionChoices.Commands.Create;
using SurveyUs.Application.Features.QuestionChoices.Queries.GetById;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class QuestionChoicesProfile : Profile
    {
        public QuestionChoicesProfile()
        {
            CreateMap<CreateQuestionChoicesCommand, QuestionChoices>().ReverseMap();
            CreateMap<GetQuestionChoicesByIdResponse, QuestionChoices>().ReverseMap();
        }
    }
}
