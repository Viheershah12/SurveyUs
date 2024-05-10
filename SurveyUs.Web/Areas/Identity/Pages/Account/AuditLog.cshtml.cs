using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SurveyUs.Application.DTOs.Logs;
using SurveyUs.Application.Features.Logs.Queries.GetCurrentUserLogs;
using SurveyUs.Application.Interfaces.Shared;
using SurveyUs.Web.Abstractions;

namespace SurveyUs.Web.Areas.Identity.Pages.Account
{
    public class AuditLogModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IAuthenticatedUserService _userService;
        public List<AuditLogResponse> AuditLogResponses;
        private IViewRenderService _viewRenderer;

        public AuditLogModel(IMediator mediator, IAuthenticatedUserService userService, IViewRenderService viewRenderer)
        {
            _mediator = mediator;
            _userService = userService;
            _viewRenderer = viewRenderer;
        }

        public async Task OnGet()
        {
            var response = await _mediator.Send(new GetAuditLogsQuery() { userId = _userService.UserId });
            AuditLogResponses = response.Data;
        }
    }
}