using System.ComponentModel.DataAnnotations;

namespace SurveyUs.Application.DTOs.Identity
{
    public class ForgotPasswordRequest
    {
        [Required] [EmailAddress] public string Email { get; set; }
    }
}