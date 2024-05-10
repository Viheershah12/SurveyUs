using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.UserExtension.Commands.Update
{
    public class UpdateUserExtensionCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
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
        public bool IsDeleted { get; set; }
        public int Store { get; set; }
        public bool HeinekenTestStatus { get; set; }
        public bool GuinnessTestStatus { get; set; }
        public bool TheoryTestStatus { get; set; }

        public class UpdateUserExtensionCommandHandler : IRequestHandler<UpdateUserExtensionCommand, Result<int>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IUserExtensionRepository _userExtensionRepository;

            public UpdateUserExtensionCommandHandler(IUserExtensionRepository userExtensionRepository,
                IUnitOfWork unitOfWork)
            {
                _userExtensionRepository = userExtensionRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(UpdateUserExtensionCommand command,
                CancellationToken cancellationToken)
            {
                var userExtension = await _userExtensionRepository.GetByIdAsync(command.Id);

                if (userExtension == null)
                {
                    return Result<int>.Fail("UserExtension Not Found.");
                }

                userExtension.UserId = command.UserId ?? userExtension.UserId;
                userExtension.Email = command.Email ?? userExtension.Email;
                userExtension.Name = command.Name ?? userExtension.Name;
                userExtension.Ic = command.Ic ?? userExtension.Ic;
                userExtension.Gender = command.Gender ?? userExtension.Gender;
                userExtension.Telephone = command.Telephone ?? userExtension.Telephone;
                userExtension.Outlet = command.Outlet ?? userExtension.Outlet;
                userExtension.OutletAddress = command.OutletAddress ?? userExtension.OutletAddress;
                userExtension.OutletLocation = command.OutletLocation ?? userExtension.OutletLocation;
                userExtension.JoiningAs = command.JoiningAs ?? userExtension.JoiningAs;
                userExtension.UniformSize = command.UniformSize ?? userExtension.UniformSize;
                userExtension.Designation = command.Designation ?? userExtension.Designation;
                userExtension.IsVerified = command.IsVerified;
                userExtension.IsDeleted = command.IsDeleted;
                userExtension.Store = command.Store == 0 ? userExtension.Store : command.Store;
                userExtension.HeinekenTestStatus = command.HeinekenTestStatus;
                userExtension.GuinnessTestStatus = command.GuinnessTestStatus;
                userExtension.TheoryTestStatus = command.TheoryTestStatus;
                await _userExtensionRepository.UpdateAsync(userExtension);
                await _unitOfWork.Commit(cancellationToken);
                return Result<int>.Success(userExtension.Id);
            }
        }
    }
}