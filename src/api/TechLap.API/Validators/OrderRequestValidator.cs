using FluentValidation;
using TechLap.API.DTOs.Requests;

namespace TechLap.API.Validators
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator()
        {
            RuleFor(o => o.Payment)
                .IsInEnum()
                .WithMessage("Invalid payment method.");

            RuleFor(o => o.Status)
                .IsInEnum()
                .WithMessage("Invalid order status.");

            RuleFor(o => o.OrderDetails)
                .NotEmpty()
                .WithMessage("OrderDetails cannot be empty.")
                .Must(HaveValidOrderDetails)
                .WithMessage("At least one order detail must have a valid ProductId and Quantity.");

            RuleForEach(o => o.OrderDetails).SetValidator(new OrderDetailRequestValidator());
        }

        private bool HaveValidOrderDetails(List<OrderDetailRequest> orderDetails)
        {
            return orderDetails.Any(od => od.ProductId > 0 && od.Quantity > 0);
        }
    }

    public class OrderDetailRequestValidator : AbstractValidator<OrderDetailRequest>
    {
        public OrderDetailRequestValidator()
        {
            RuleFor(od => od.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than 0.");
        }
    }

    public class OrderAdminRequestValidator : AbstractValidator<OrderAdminRequest>
    {
        public OrderAdminRequestValidator()
        {
            RuleFor(o => o.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be greater than 0.");
            RuleFor(o => o.Payment)
                .IsInEnum()
                .WithMessage("Invalid payment method.");

            RuleFor(o => o.Status)
                .IsInEnum()
                .WithMessage("Invalid order status.");

            RuleFor(o => o.OrderDetails)
                .NotEmpty()
                .WithMessage("OrderDetails cannot be empty.")
                .Must(HaveValidOrderDetails)
                .WithMessage("At least one order detail must have a valid ProductId and Quantity.");

            RuleForEach(o => o.OrderDetails).SetValidator(new OrderDetailRequestValidator());
        }
        private bool HaveValidOrderDetails(List<OrderDetailRequest> orderDetails)
        {
            return orderDetails.Any(od => od.ProductId > 0 && od.Quantity > 0);
        }
    }
}
