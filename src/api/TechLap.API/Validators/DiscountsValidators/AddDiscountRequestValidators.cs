using FluentValidation;
using TechLap.API.DTOs.Requests.DiscountRequests;

namespace TechLap.API.Validators.DiscountsValidators;

public class AddDiscountRequestValidators : AbstractValidator<AddAdminDiscountRequest>
{
    public AddDiscountRequestValidators()
    {
        // RuleFor(o => o.DiscountCode)
        //     .NotEmpty()
        //     .WithMessage("{DiscountCode} is required")
        //     .MaximumLength(70)
        //     .WithMessage("{DiscountCode} must not exceed 3 characters");
        //
        // RuleFor(o => o.DiscountPercentage)
        //     .NotEmpty()
        //     .WithMessage("{DiscountPercentage} is required");
        // RuleFor(o => o.EndDate)
        //     .NotEmpty()
        //     .WithMessage("{EndDate} is required")
        //     .GreaterThan(DateTime.Now)
        //     .WithMessage("{EndDate} must be greater than the current date and time");
        // RuleFor(o => o.UsageLimit)
        //     .NotEmpty()
        //     .WithMessage("{UsageLimit} is required");

       
    }
    
    
}