using FluentValidation;
using TechLap.API.DTOs.Requests.DiscountRequests;

namespace TechLap.API.Validators.DiscountsValidators;

public class ApplyDiscountValidators : AbstractValidator<ApplyUserDiscountRequest>
{
    public ApplyDiscountValidators()
    {
        RuleFor(o => o.DiscountCode)
            .NotEmpty()
            .WithMessage("{DiscountCode} is required")
            .MaximumLength(70)
            .WithMessage("{DiscountCode} must not exceed 3 characters");

    }
}