using AutoMapper;
using SurveyUs.Application.Features.QuestionMappings.Commands.Create;
using SurveyUs.Application.Features.QuestionMappings.Commands.Update;
using SurveyUs.Application.Features.QuestionMappings.Queries.GetAllCached;
using SurveyUs.Application.Features.QuestionMappings.Queries.Queries;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Mappings
{
    public class QuestionMappingsProfile : Profile
    {
        public QuestionMappingsProfile()
        {
            CreateMap<GetQuestionMappingsByIdResponse, QuestionMappings>().ReverseMap();
            CreateMap<GetAllQuestionMappingsCachedResponse, QuestionMappings>().ReverseMap();
            CreateMap<CreateQuestionMappingCommandHandler, QuestionMappings>().ReverseMap();
            CreateMap<UpdateQuestionMappingCommand.UpdateQuestionMappingCommandHandler, QuestionMappings>().ReverseMap();
        }

    }
}
