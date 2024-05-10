using AutoMapper;
using SurveyUs.Application.Features.Question.Commands.Create;
using SurveyUs.Application.Features.Question.Commands.Update;
using SurveyUs.Application.Features.Question.Queries.GetAllCached;
using SurveyUs.Application.Features.Question.Queries.GetById;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<CreateQuestionCommand, Question>().ReverseMap();
            CreateMap<GetQuestionByIdResponse, Question>().ReverseMap();
            CreateMap<GetAllQuestionCachedResponse, Question>().ReverseMap();
            CreateMap<UpdateQuestionCommand, Question>().ReverseMap();
        }
    }
}
