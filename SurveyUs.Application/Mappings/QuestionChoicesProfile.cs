using AutoMapper;
using SurveyUs.Application.Features.QuestionChoices.Commands.Create;
using SurveyUs.Application.Features.QuestionChoices.Commands.Update;
using SurveyUs.Application.Features.QuestionChoices.Queries.GetAllCached;
using SurveyUs.Application.Features.QuestionChoices.Queries.GetById;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    public class QuestionChoicesProfile : Profile
    {
        public QuestionChoicesProfile()
        {
            CreateMap<GetQuestionChoicesByIdResponse, QuestionChoices>().ReverseMap();
            CreateMap<GetAllQuestionChoicesCachedResponse, QuestionChoices>().ReverseMap();
            CreateMap<CreateQuestionChoicesCommand, QuestionChoices>().ReverseMap();
            CreateMap<UpdateQuestionChoicesCommand, QuestionChoices>().ReverseMap();
        }
    }
}
