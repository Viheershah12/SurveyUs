using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.QuestionCategoryMapping.Commands.Create
{
    public class CreateQuestionCategoryMappingCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public int QuestionMappingId { get; set; }

        public int QuestionCategoryId { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class CreateQuestionCategoryMappingCommandHandler : IRequestHandler<CreateQuestionCategoryMappingCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IQuestionCategoryMappingRepository _questionCategoryMappingRepository;
        private IUnitOfWork _unitOfWork { get; }

        public CreateQuestionCategoryMappingCommandHandler(IQuestionCategoryMappingRepository questionCategoryMappingRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _questionCategoryMappingRepository = questionCategoryMappingRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateQuestionCategoryMappingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var questionCategory = _mapper.Map<Domain.Entities.QuestionCategoryMapping>(request);

                await _questionCategoryMappingRepository.InsertAsync(questionCategory);
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
