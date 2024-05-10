using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionCategory.Commands.Update
{
    public class UpdateQuestionCategoryCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string UpdatedBy {  get; set; }


        public class UpdateQuestionCategoryCommandHandler : IRequestHandler<UpdateQuestionCategoryCommand, Result<string>>
        {
            private readonly IQuestionCategoryRepository _questionCategoryRepository;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateQuestionCategoryCommandHandler(IQuestionCategoryRepository questionCategoryRepository, IUnitOfWork unitOfWork)
            {
                _questionCategoryRepository = questionCategoryRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<string>> Handle(UpdateQuestionCategoryCommand command, CancellationToken cancellationToken)
            {
                var category = await _questionCategoryRepository.GetByIdAsync(command.Id);

                if (category == null)
                {
                    return Result<string>.Fail("Question Category Not Found");
                }

                category.CategoryName = command.CategoryName;
                category.UpdatedOn = DateTime.Now;
                category.UpdatedBy = command.UpdatedBy;

                await _questionCategoryRepository.UpdateAsync(category);
                await _unitOfWork.Commit(cancellationToken);
                return Result<string>.Success(data: category.CategoryName);
            }
        }
    }
}
