using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionMedia.Commands.Create
{
    public class CreateQuestionMediaCommand : IRequest<Result<Domain.Entities.QuestionMedia>>
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public string Name { get; set; }
        public byte[] FileBinary { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int CampaignId { get; set; }
        public int StoreId { get; set; }
        public string UserId { get; set; }
    }

    public class CreateQuestionMediaCommandHandler : IRequestHandler<CreateQuestionMediaCommand, Result<Domain.Entities.QuestionMedia>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionMediaRepository _questionMediaRepository;
        private IUnitOfWork _unitOfWork { get; }

        public CreateQuestionMediaCommandHandler(
            IMapper mapper,
            IQuestionMediaRepository questionMediaRepository,
            IUnitOfWork unitOfWork
        )
        {
            _mapper = mapper;
            _questionMediaRepository = questionMediaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Domain.Entities.QuestionMedia>> Handle(CreateQuestionMediaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var question = _mapper.Map<Domain.Entities.QuestionMedia>(request);
                await _questionMediaRepository.InsertAsync(question);
                await _unitOfWork.Commit(cancellationToken);

                return Result<Domain.Entities.QuestionMedia>.Success(question);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
