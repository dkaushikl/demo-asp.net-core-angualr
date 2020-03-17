namespace Demo.API.Models
{
    using FluentValidation;

    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        public RegisterModelValidator(RegisterModel model)
        {
            this.RuleFor(x => x.FirstName).NotEmpty().NotNull().WithMessage("First Name is required.");

            this.RuleFor(x => x.LastName).NotEmpty().NotNull().WithMessage("Last Name is required.");

            this.RuleFor(x => x.Email).EmailAddress().NotEmpty().NotNull().WithMessage("Email is required.");

            this.RuleFor(x => x.Password).NotEmpty().NotNull().WithMessage("Password is required.");

            this.RuleFor(x => x.ConfirmPassword).NotEmpty().NotNull().WithMessage("Confirm Password is required.");

            this.RuleFor(x => x.ConfirmPassword).Equals(model.Password);
        }
    }
}