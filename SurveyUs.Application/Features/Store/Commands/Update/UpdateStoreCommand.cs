using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Enums;

namespace SurveyUs.Application.Features.Store.Commands.Update
{
    public class UpdateStoreCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public StateEnum State { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public class UpdateProductCommandHandler : IRequestHandler<UpdateStoreCommand, Result<int>>
        {
            private readonly IStoreRepository _storeRepository;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateProductCommandHandler(IStoreRepository storeRepository, 
                IUnitOfWork unitOfWork)
            {
                _storeRepository = storeRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateStoreCommand command, CancellationToken cancellationToken)
            {
                var store = await _storeRepository.GetByIdAsync(command.Id);

                if (store == null)
                {
                    return Result<int>.Fail("Store Not Found.");
                }

                store.State = command.State != default ? command.State : store.State;
                store.Name = !string.IsNullOrEmpty(command.Name) ? command.Name : store.Name;
                store.Line1 = !string.IsNullOrEmpty(command.Line1) ? command.Line1 : store.Line1;
                store.Line2 = !string.IsNullOrEmpty(command.Line2) ? command.Line2 : store.Line2;
                store.UpdatedOn = DateTime.Now;
                store.UpdatedBy = command.UpdatedBy ?? store.UpdatedBy;
                store.IsActive = command.IsActive;
                await _storeRepository.UpdateAsync(store);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(store.Id);
            }
        }
    }
}