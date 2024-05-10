using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.UserExtension.Commands.Delete
{
    public class DeleteUserExtensionCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public class DeleteUserExtensionCommandHandler : IRequestHandler<DeleteUserExtensionCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IUserExtensionRepository _userExtensionRepository;

            public DeleteUserExtensionCommandHandler(IUserExtensionRepository userExtensionRepository,
                IUnitOfWork unitOfWork)
            {
                _userExtensionRepository = userExtensionRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(DeleteUserExtensionCommand command,
                CancellationToken cancellationToken)
            {
                var userExtension = await _userExtensionRepository.GetByIdAsync(command.Id);
                await _userExtensionRepository.DeleteAsync(userExtension);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(userExtension.Id);
            }
        }
    }
}