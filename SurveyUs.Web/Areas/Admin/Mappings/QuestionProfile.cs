using AutoMapper;
using SurveyUs.Application.Features.Question.Commands.Create;
using SurveyUs.Application.Features.Question.Queries.GetAllCached;
using SurveyUs.Application.Features.Question.Queries.GetById;
using SurveyUs.Domain.Entities;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Admin.Mappings
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<Question, QuestionViewModel>()
                .ForMember(x => x.QuestionTypeDropdown, y => y.Ignore())
                .ForMember(x => x.QuestionCategoryDropdown, y => y.Ignore())
                .ForMember(x => x.Options, y => y.Ignore())
                .ReverseMap();
            
            
            CreateMap<CreateQuestionCommand, QuestionViewModel>()
                .ForMember(x => x.QuestionTypeDropdown, y => y.Ignore())
                .ForMember(x => x.QuestionCategoryDropdown, y => y.Ignore())
                .ForMember(x => x.Options, y => y.Ignore())
                .ReverseMap();


            CreateMap<GetAllQuestionCachedResponse, QuestionViewModel>()
                .ForMember(x => x.QuestionTypeDropdown, y => y.Ignore())
                .ForMember(x => x.QuestionCategoryDropdown, y => y.Ignore())
                .ForMember(x => x.Options, y => y.Ignore())
                .ReverseMap();

            CreateMap<GetQuestionByIdResponse, QuestionViewModel>().ReverseMap();
        }
    }
}
