using FluentValidation;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Admin.Validators
{
    public class ParticipantViewModelValidator : AbstractValidator<ParticipantViewModel>
    {
        public ParticipantViewModelValidator()
        {
            RuleFor(p => p.FirstName)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).+$").WithMessage("Your password must contain at least one lowercase letter, one uppercase letter, and one number.");

            RuleFor(p => p.ConfirmPassword)
                .Equal(p => p.Password)
                .WithMessage("Passwords do not match");

            RuleFor(p => p.Ic)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .Matches("^[a-zA-Z\\d]{6,}$").WithMessage("IC/Passport must be a combination of at least 6 alphanumeric characters (letters A-Z, a-z, and digits 0-9). Special characters and spaces are not allowed.");

            RuleFor(p => p.Telephone)
               .NotEmpty().WithMessage("{PropertyName} is required.")
               .Matches("^[0-9]+$").WithMessage("{PropertyName} field must contain only numbers.");

            RuleFor(p => p.OutletName)
               .NotEmpty().WithMessage("{PropertyName} is required.")
               .NotNull();

            RuleFor(p => p.OutletAddress)
               .NotEmpty().WithMessage("{PropertyName} is required.")
               .NotNull();
        }
    }
}