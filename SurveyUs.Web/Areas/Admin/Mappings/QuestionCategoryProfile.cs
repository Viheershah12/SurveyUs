using AutoMapper;
using SurveyUs.Application.Features.QuestionCategory.Commands.Create;
using SurveyUs.Application.Features.QuestionCategory.Commands.Update;
using SurveyUs.Application.Features.QuestionCategory.Query.GetAllCached;
using SurveyUs.Domain.Entities;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class QuestionCategoryProfile : Profile
    {
        public QuestionCategoryProfile()
        {
            CreateMap<CreateQuestionCategoryCommand, QuestionCategoryViewModel>().ReverseMap();
            CreateMap<QuestionCategory, QuestionCategoryViewModel>().ReverseMap();
            CreateMap<GetAllQuestionCategoryCachedResponse, QuestionCategoryViewModel>().ReverseMap();
            CreateMap<UpdateQuestionCategoryCommand, QuestionCategoryViewModel>().ReverseMap();
        }
    }
}
