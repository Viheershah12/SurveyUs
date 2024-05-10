using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.StoreMappings.Commands.Create
{
    public class CreateStoreMappingsCommand : IRequest<Result<int>>
    {
        public int StoreId { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class CreateStoreMappingsCommandHandler : IRequestHandler<CreateStoreMappingsCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStoreMappingsRepository _storeMappingsRepository;

        public CreateStoreMappingsCommandHandler(IStoreMappingsRepository storeMappingsRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _storeMappingsRepository = storeMappingsRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private IUnitOfWork _unitOfWork { get; }

        public async Task<Result<int>> Handle(CreateStoreMappingsCommand request, CancellationToken cancellationToken)
        {
            var storeMapping = _mapper.Map<Domain.Entities.StoreMappings>(request);

            await _storeMappingsRepository.InsertAsync(storeMapping);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(storeMapping.Id);
        }
    }
}
