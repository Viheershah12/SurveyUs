using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.StoreMappings.Commands.Delete
{
    public class DeleteStoreMappingsCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteStoreMappingsCommandHandler : IRequestHandler<DeleteStoreMappingsCommand, Result<int>>
        {
            private readonly IStoreMappingsRepository _storeMappingsRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteStoreMappingsCommandHandler(IStoreMappingsRepository storeMappingsRepository, IUnitOfWork unitOfWork)
            {
                _storeMappingsRepository = storeMappingsRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteStoreMappingsCommand command, CancellationToken cancellationToken)
            {
                var storeMapping = await _storeMappingsRepository.GetByIdAsync(command.Id);
                //await _storeMappingsRepository.UpdateAsync(storeMapping);
                //storeMapping.IsDeleted = true;
                await _storeMappingsRepository.DeleteAsync(storeMapping);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(storeMapping.Id);
            }
        }
    }
}
