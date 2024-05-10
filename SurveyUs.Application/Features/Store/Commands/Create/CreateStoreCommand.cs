using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Domain.Enums;

namespace SurveyUs.Application.Features.Store.Commands.Create
{
    public class CreateStoreCommand : IRequest<Result<int>>
    {
        public string Line1 { get; set; }
        public string? Line2 { get; set; }
        public StateEnum State { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set;}
    }

    public class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStoreRepository _storeRepository;

        public CreateStoreCommandHandler(IStoreRepository storeRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private IUnitOfWork _unitOfWork { get; }

        public async Task<Result<int>> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
        {
            var store = _mapper.Map<Domain.Entities.Store>(request);

            store.CreatedOn = DateTime.Now;
            store.UpdatedOn = DateTime.Now;

            await _storeRepository.InsertAsync(store);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(store.Id);
        }
    }
}