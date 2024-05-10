using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyUs.Application.DTOs.Logs;

namespace SurveyUs.Application.Interfaces.Repositories
{
    public interface ILogRepository
    {
        Task<List<AuditLogResponse>> GetAuditLogsAsync(string userId);

        Task AddLogAsync(string action, string userId);
    }
}