namespace Demo.API.Models
{
    using FluentValidation;

    public class ResetPasswordViewModelValidator : AbstractValidator<ResetPasswordViewModel>
    {
        public ResetPasswordViewModelValidator(ResetPasswordViewModel model)
        {
            this.RuleFor(x => x.Password).NotEmpty().NotNull().WithMessage("Password is required.");

            this.RuleFor(x => x.ConfirmPassword).NotEmpty().NotNull().WithMessage("Confirm Password is required.");

            this.RuleFor(x => x.ConfirmPassword).Equals(model.Password);
        }
    }
}