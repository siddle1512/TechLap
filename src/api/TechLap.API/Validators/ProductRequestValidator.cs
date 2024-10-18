using FluentValidation;
using TechLap.API.DTOs.Requests;

namespace TechLap.API.Validators
{
    public class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(o => o.Model)
                .NotNull()
                .MaximumLength(70)
                .WithMessage("{Model} must not exceed 70 characters");

            RuleFor(o => o.Brand)
                .NotNull()
                .MaximumLength(70)
                .WithMessage("{Brand} must not exceed 70 characters");

            RuleFor(o => o.CategoryId)
                .NotNull();

            RuleFor(o => o.Cpu)
                .NotNull()
                .MaximumLength(70)
                .WithMessage("{Cpu} must not exceed 70 characters");

            RuleFor(o => o.Ram)
                .NotNull()
                .MaximumLength(70)
                .WithMessage("{Ram} must not exceed 70 characters");

            RuleFor(o => o.Vga)
                .NotNull()
                .MaximumLength(70)
                .WithMessage("{Vga} must not exceed 70 characters");

            RuleFor(o => o.ScreenSize)
                .NotNull()
                .MaximumLength(70)
                .WithMessage("{ScreenSize} must not exceed 70 characters");

           RuleFor(o => o.HardDisk)
                .NotNull()
                .MaximumLength(70)
                .WithMessage("{HardDisk} must not exceed 70 characters");

            RuleFor(o => o.Os)
                .NotNull()
                .MaximumLength(70)
                .WithMessage("{Os} must not exceed 70 characters");

            RuleFor(o => o.Price)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("{Price} must be greater than zero.");

            RuleFor(o => o.Stock)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .WithMessage("{Stock} must be greater than or equal to zero.");
        }
    }
}
