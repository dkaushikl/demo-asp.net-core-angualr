namespace Demo.API.Models
{
    using FluentValidation;

    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            this.RuleFor(x => x.Email).EmailAddress().NotEmpty().NotNull().WithMessage("Email is required.");

            this.RuleFor(x => x.Password).NotEmpty().NotNull().WithMessage("Password is required.");
        }
    }
}