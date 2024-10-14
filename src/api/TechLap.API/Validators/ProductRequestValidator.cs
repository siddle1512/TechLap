using FluentValidation;
using TechLap.API.DTOs.Requests;

namespace TechLap.API.Validators
{
    public class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(o => o.Model)
                .MaximumLength(70)
                .WithMessage("{Model} must not exceed 70 characters");
        }
    }
}
