using AutoMapper;
using SurveyUs.Application.Features.QuestionCategoryMapping.Commands.Create;
using SurveyUs.Application.Features.QuestionCategoryMapping.Queries.GetAllCached;
using SurveyUs.Application.Features.QuestionCategoryMapping.Queries.GetById;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    public class QuestionCategoryMappingProfile : Profile
    {
        public QuestionCategoryMappingProfile()
        {
            CreateMap<CreateQuestionCategoryMappingCommand, QuestionCategoryMapping>().ReverseMap();
            CreateMap<GetQuestionCategoryMappingByIdResponse, QuestionCategoryMapping>().ReverseMap();
            CreateMap<GetAllQuestionCategoryMappingCachedResponse, QuestionCategoryMapping>().ReverseMap();
        }
    }
}
