using AutoMapper;
using SurveyUs.Application.Features.QuestionAnswers.Commands.Create;
using SurveyUs.Application.Features.QuestionAnswers.Queries.GetAll;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    public class QuestionAnswerProfile : Profile
    {
        public QuestionAnswerProfile()
        {
            CreateMap<CreateQuestionAnswerCommand, QuestionAnswers>().ReverseMap();
            CreateMap<GetQuestionAnswersByQuestionIdResponse, QuestionAnswers>().ReverseMap();
        }
    }
}
