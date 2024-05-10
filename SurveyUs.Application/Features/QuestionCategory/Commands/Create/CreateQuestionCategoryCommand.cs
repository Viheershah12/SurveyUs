using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionCategory.Commands.Create
{
    public class CreateQuestionCategoryCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class CreateQuestionCategoryCommandHandler : IRequestHandler<CreateQuestionCategoryCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionCategoryRepository _questionCategoryRepository;
        private IUnitOfWork _unitOfWork { get; }

        public CreateQuestionCategoryCommandHandler(IQuestionCategoryRepository questionCategoryRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _questionCategoryRepository = questionCategoryRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateQuestionCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var questionCategory = _mapper.Map<Domain.Entities.QuestionCategory>(request);

                await _questionCategoryRepository.InsertAsync(questionCategory);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(questionCategory.Id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

}
