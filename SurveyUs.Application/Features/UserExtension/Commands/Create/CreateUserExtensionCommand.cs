using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.UserExtension.Commands.Create
{
    public class CreateUserExtensionCommand : IRequest<Result<int>>
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Ic { get; set; }
        public string Gender { get; set; }
        public string Telephone { get; set; }
        public string Outlet { get; set; }
        public string OutletAddress { get; set; }
        public string OutletLocation { get; set; }
        public string JoiningAs { get; set; }
        public string UniformSize { get; set; }
        public string Designation { get; set; }
        public bool IsVerified { get; set; }
        public int Store { get; set; }
        public bool HeinekenTestStatus { get; set; }
        public bool GuinnessTestStatus { get; set; }
        public bool TheoryTestStatus { get; set; }
    }

    public class CreateUserExtensionCommandHandler : IRequestHandler<CreateUserExtensionCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUserExtensionRepository _userExtensionRepository;

        public CreateUserExtensionCommandHandler(IUserExtensionRepository userExtensionRepository,
            IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userExtensionRepository = userExtensionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private IUnitOfWork _unitOfWork { get; }

        public async Task<Result<int>> Handle(CreateUserExtensionCommand request, CancellationToken cancellationToken)
        {
            var userExtension = _mapper.Map<Domain.Entities.UserExtension>(request);
            await _userExtensionRepository.InsertAsync(userExtension);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(userExtension.Id);
        }
    }
}