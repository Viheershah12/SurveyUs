using AutoMapper;
using SurveyUs.Application.Features.UserAnswers.Commands.Create;
using SurveyUs.Application.Features.UserAnswers.Queries.GetById;
using SurveyUs.Domain.Entities;
using SurveyUs.Web.Areas.Questions.Model;

namespace SurveyUs.Web.Areas.Questions.Mappings
{
    public class UserAnswersProfile : Profile
    {
        public UserAnswersProfile()
        {
            CreateMap<CreateUserAnswersCommand, UserAnswers>().ReverseMap();
            CreateMap<GetUserAnswersByIdResponse, QuestionsModel>().ReverseMap();
            CreateMap<GetUserAnswersByIdResponse, CreateUserAnswersCommand>().ReverseMap();
        }
    }
}
