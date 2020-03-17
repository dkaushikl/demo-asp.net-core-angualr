namespace Demo.API.Models
{
    using FluentValidation;

    public class ForgotPasswordViewModelValidator : AbstractValidator<ForgotPasswordViewModel>
    {
        public ForgotPasswordViewModelValidator(ForgotPasswordViewModel model)
        {
            this.RuleFor(x => x.Email).EmailAddress().NotEmpty().NotNull().WithMessage("Email is required.");
        }
    }
}