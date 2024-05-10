using AutoMapper;
using SurveyUs.Application.Features.UserAnswers.Commands.Create;
using SurveyUs.Application.Features.UserAnswers.Queries.GetAllCached;
using SurveyUs.Application.Features.UserAnswers.Queries.GetById;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    public class UserAnswersProfile : Profile
    {
        public UserAnswersProfile()
        {
            CreateMap<GetUserAnswersByIdResponse, UserAnswers>().ReverseMap();
            CreateMap<GetAllUserAnswersCachedResponse, UserAnswers>().ReverseMap();
            CreateMap<CreateUserAnswersCommand, UserAnswers>().ReverseMap();
        }
    }
}
