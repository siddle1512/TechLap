using FluentValidation;
using TechLap.API.DTOs.Requests;

namespace TechLap.API.Validators
{
    public class UserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public UserRequestValidator()
        {
            RuleFor(o => o.FullName)
                .NotNull()
                .WithMessage("{FullName} is required.")
                .MaximumLength(100)
                .WithMessage("{FullName} must not exceed 255 characters");
            RuleFor(o => o.Email)
                .NotNull()
                .WithMessage("{Email} is required.")
                .EmailAddress()
                .WithMessage("{Email} is not an email address");
            RuleFor(o => o.PhoneNumber)
                .NotNull()
                .WithMessage("{PhoneNumber} is required.")
                .Matches(@"^0[1-9]\d{8}$")
                .WithMessage("{PhoneNumber} is not true fomat");
            RuleFor(o => o.HashedPassword)
                .NotNull()
                .WithMessage("{Password} is required.")
                .Matches(@"(?=.*[A-Z])(?=.*[\W_])")
                .WithMessage("{Password} must contain at least one uppercase letter and one special character.");
            RuleFor(o => o.BirthYear)
                .NotNull()
                .WithMessage("{Birthday} is required.")
                .Must(date => date <= DateTime.Now)
                .WithMessage("{Birthday} cannot be in the future.");
        }
    }
}
