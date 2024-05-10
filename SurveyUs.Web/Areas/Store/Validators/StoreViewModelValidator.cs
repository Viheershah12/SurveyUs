using FluentValidation;
using SurveyUs.Web.Areas.Admin.Models;

namespace SurveyUs.Web.Areas.Store.Validators
{
    public class StoreViewModelValidator : AbstractValidator<StoreViewModel>
    {
        public StoreViewModelValidator()
        {
            RuleFor(p => p.Line1)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(p => p.State)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();

            //RuleFor(p => p.StartDate)
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .NotNull();

            //RuleFor(p => p.EndDate)
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .NotNull();
        }
    }
}