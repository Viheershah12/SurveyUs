using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionMedia.Queries.GetById
{
    public class GetQuestionMediaByQuestionIdQuery : IRequest<Result<List<GetQuestionMediaByIdResponse>>>
    {
        public int QuestionId { get; set; }
        public int CampaignId { get; set; }
        public int StoreId { get; set; }
        public string UserId { get; set; }

        public class GetQuestionMediaByQuestionIdQueryHandler : IRequestHandler<GetQuestionMediaByQuestionIdQuery, Result<List<GetQuestionMediaByIdResponse>>>
        {
            private readonly IMapper _mapper;
            private readonly IQuestionMediaRepository _questionMediaRepository;

            public GetQuestionMediaByQuestionIdQueryHandler(
                IMapper mapper,
                IQuestionMediaRepository questionMediaRepository
            )
            {
                _mapper = mapper;
                _questionMediaRepository = questionMediaRepository;
            }

            public async Task<Result<List<GetQuestionMediaByIdResponse>>> Handle(GetQuestionMediaByQuestionIdQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var questionMedia = await _questionMediaRepository.GetByQuestionId(request.QuestionId, request.CampaignId, request.StoreId, request.UserId);
                    var mappedQuestionMedia = _mapper.Map<List<GetQuestionMediaByIdResponse>>(questionMedia);

                    return Result<List<GetQuestionMediaByIdResponse>>.Success(mappedQuestionMedia);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException();
                }
            }
        }
    }
}
