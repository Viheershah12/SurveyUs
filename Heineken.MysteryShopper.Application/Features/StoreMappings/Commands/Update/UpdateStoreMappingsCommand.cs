using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.StoreMappings.Commands.Update
{
    public class UpdateStoreMappingsCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string UserId { get; set; }
        public bool isDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class UpdateStoreMappingsCommandHandler : IRequestHandler<UpdateStoreMappingsCommand, Result<int>>
    {
        private readonly IStoreMappingsRepository _storeMappingsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStoreMappingsCommandHandler(IStoreMappingsRepository storeMappingsRepository, IUnitOfWork unitOfWork)
        {
            _storeMappingsRepository = storeMappingsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(UpdateStoreMappingsCommand command, CancellationToken cancellationToken)
        {
            var storeMapping = await _storeMappingsRepository.GetByIdAsync(command.Id);

            if (storeMapping == null)
            {
                return Result<int>.Fail("StoreMapping Not Found.");
            }

            storeMapping.IsDeleted = command.isDeleted;
            storeMapping.UpdatedBy = command.UpdatedBy ?? storeMapping.UpdatedBy;
            
            await _storeMappingsRepository.UpdateAsync(storeMapping);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(storeMapping.Id);
        }
    }
}
