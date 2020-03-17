using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Demo.API.Models
{
    public class ChangePasswordModel
    {
        [DataType(DataType.Password), Required(ErrorMessage = "Old Password Required")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "New Password Required")]
        public string NewPassword { get; set; }
    }

    public class ChangePasswordModelValidator : AbstractValidator<ChangePasswordModel>
    {
        public ChangePasswordModelValidator(ChangePasswordModel model)
        {
            this.RuleFor(x => x.OldPassword).NotEmpty().NotNull().WithMessage("Old password is required.");

            this.RuleFor(x => x.NewPassword).NotEmpty().NotNull().WithMessage("New password is required.");
        }
    }
}
