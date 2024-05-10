using AutoMapper;
using SurveyUs.Application.Features.QuestionMedia.Commands.Create;
using SurveyUs.Application.Features.QuestionMedia.Commands.Update;
using SurveyUs.Application.Features.QuestionMedia.Queries.GetAllCached;
using SurveyUs.Application.Features.QuestionMedia.Queries.GetById;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    public class QuestionMediaProfile : Profile
    {
        public QuestionMediaProfile()
        {
            CreateMap<CreateQuestionMediaCommand, QuestionMedia>().ReverseMap();
            CreateMap<UpdateQuestionMediaCommand, QuestionMedia>().ReverseMap();
            CreateMap<GetQuestionMediaByIdResponse, QuestionMedia>().ReverseMap();
            CreateMap<GetAllQuestionMediaCacheResponse, QuestionMedia>().ReverseMap();
        }
    }
}
