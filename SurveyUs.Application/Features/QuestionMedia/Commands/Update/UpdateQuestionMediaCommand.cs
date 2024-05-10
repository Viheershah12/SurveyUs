using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionMedia.Commands.Update
{
    public class UpdateQuestionMediaCommand : IRequest<Result<Domain.Entities.QuestionMedia>>
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

    public class UpdateQuestionMediaCommandHandler : IRequestHandler<UpdateQuestionMediaCommand, Result<Domain.Entities.QuestionMedia>>
    {
        private readonly IQuestionMediaRepository _questionMediaRepository;

        private IUnitOfWork _unitOfWork;

        public UpdateQuestionMediaCommandHandler(IQuestionMediaRepository questionMediaRepository)
        {
            _questionMediaRepository = questionMediaRepository;
        }

        public async Task<Result<Domain.Entities.QuestionMedia>> Handle(UpdateQuestionMediaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var questionMedia = await _questionMediaRepository.GetByIdAsync(request.Id);

                if (questionMedia == null)
                {
                    return Result<Domain.Entities.QuestionMedia>.Fail("QuestionMedia Not Found.");
                }

                questionMedia.IsDeleted = request.IsDeleted;
                questionMedia.LastModifiedBy = request.LastModifiedBy;
                questionMedia.LastModifiedOn = request.LastModifiedOn;

                await _questionMediaRepository.UpdateAsync(questionMedia);
                await _unitOfWork.Commit(cancellationToken);

                return Result<Domain.Entities.QuestionMedia>.Success(questionMedia);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
