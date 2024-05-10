using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.Store.Commands.Delete
{
    public class DeleteStoreCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommand, Result<int>>
        {
            private readonly IStoreRepository _storeRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteStoreCommandHandler(IStoreRepository storeRepository, IUnitOfWork unitOfWork)
            {
                _storeRepository = storeRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteStoreCommand command, CancellationToken cancellationToken)
            {
                var store = await _storeRepository.GetByIdAsync(command.Id);
                store.IsDeleted = true;
                await _storeRepository.UpdateAsync(store);
                //await _storeRepository.DeleteAsync(store);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(store.Id);
            }
        }
    }
}