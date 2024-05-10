using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionAnswers.Queries.GetAll
{
    public class GetQuestionAnswersByQuestionId : IRequest<Result<List<GetQuestionAnswersByQuestionIdResponse>>>
    {
        public int QuestionId { get; set; }

        public class GetQuestionAnswersByQuestionIdHandler : IRequestHandler<GetQuestionAnswersByQuestionId,
            Result<List<GetQuestionAnswersByQuestionIdResponse>>>
        {
            private readonly IQuestionAnswerRepository _questionAnswerRepository;
            private readonly IMapper _mapper;

            public GetQuestionAnswersByQuestionIdHandler(IQuestionAnswerRepository questionAnswerRepository, IMapper mapper)
            {
                _questionAnswerRepository = questionAnswerRepository;
                _mapper = mapper;
            }

            public async Task<Result<List<GetQuestionAnswersByQuestionIdResponse>>> Handle(GetQuestionAnswersByQuestionId request, CancellationToken cancellationToken)
            {
                var result = await _questionAnswerRepository.GetAllByQuestionId(request.QuestionId);

                var mappedResult = _mapper.Map<List<GetQuestionAnswersByQuestionIdResponse>>(result);
                return Result<List<GetQuestionAnswersByQuestionIdResponse>>.Success(mappedResult);
            }
        }
    }
}
