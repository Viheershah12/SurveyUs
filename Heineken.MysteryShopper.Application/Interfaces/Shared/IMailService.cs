using System.Threading.Tasks;
using SurveyUs.Application.DTOs.Mail;

namespace SurveyUs.Application.Interfaces.Shared
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}