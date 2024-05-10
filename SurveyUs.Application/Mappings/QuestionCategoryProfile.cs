using AutoMapper;
using SurveyUs.Application.Features.QuestionCategory.Commands.Create;
using SurveyUs.Application.Features.QuestionCategory.Query.GetAllCached;
using SurveyUs.Application.Features.QuestionCategory.Query.GetById;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    public class QuestionCategoryProfile : Profile
    {
        public QuestionCategoryProfile()
        {
            CreateMap<CreateQuestionCategoryCommand, QuestionCategory>().ReverseMap();
            CreateMap<GetQuestionCategoryByIdResponse, QuestionCategory>().ReverseMap();
            CreateMap<GetAllQuestionCategoryCachedResponse, QuestionCategory>().ReverseMap();
        }
    }
}
