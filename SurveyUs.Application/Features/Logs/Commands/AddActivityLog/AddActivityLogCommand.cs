using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using SurveyUs.Application.Interfaces.Repositories;

namespace SurveyUs.Application.Features.Logs.Commands.AddActivityLog
{
    public class AddActivityLogCommand : IRequest<Result<int>>
    {
        public string Action { get; set; }
        public string UserId { get; set; }
        public string Module { get; set; }
        public string PrimaryKey { get; set; }
    }

    public class AddActivityLogCommandHandler : IRequestHandler<AddActivityLogCommand, Result<int>>
    {
        private readonly ILogRepository _repo;

        public AddActivityLogCommandHandler(ILogRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        private IUnitOfWork _unitOfWork { get; }

        public async Task<Result<int>> Handle(AddActivityLogCommand request, CancellationToken cancellationToken)
        {
            await _repo.AddLogAsync(request.Action, request.UserId);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(1);
        }
    }
}