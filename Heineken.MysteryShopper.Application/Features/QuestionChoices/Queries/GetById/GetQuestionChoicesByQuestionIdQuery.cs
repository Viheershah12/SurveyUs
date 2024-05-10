using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionChoices.Queries.GetById
{
    public class GetQuestionChoicesByQuestionIdQuery : IRequest<Result<List<GetQuestionChoicesByIdResponse>>>
    {
        public int QuestionId { get; set; }
        public class GetQuestionChoicesByQuestionIdQueryHandler : IRequestHandler<GetQuestionChoicesByQuestionIdQuery, Result<List<GetQuestionChoicesByIdResponse>>>
        {
            private readonly IQuestionChoicesRepository _questionChoicesRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            public GetQuestionChoicesByQuestionIdQueryHandler(IQuestionChoicesRepository questionChoicesRepository, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _questionChoicesRepository = questionChoicesRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<Result<List<GetQuestionChoicesByIdResponse>>> Handle(GetQuestionChoicesByQuestionIdQuery query, CancellationToken cancellationToken)
            {
                try
                {
                    var choices = _questionChoicesRepository.QuestionChoices
                        .Where(q => q.QuestionId == query.QuestionId).ToList();

                    if (!choices.Any())
                        return Result<List<GetQuestionChoicesByIdResponse>>.Fail("No Question Choices Found");

                    var mappedChoices = _mapper.Map<List<GetQuestionChoicesByIdResponse>>(choices);

                    return Result<List<GetQuestionChoicesByIdResponse>>.Success(mappedChoices);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
