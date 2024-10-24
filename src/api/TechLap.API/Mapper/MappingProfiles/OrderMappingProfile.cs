using AutoMapper;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Responses.OrderDTOs;
using TechLap.API.Models;

namespace TechLap.API.Mapper.MappingProfiles
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // Mapping Response
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Payment.ToString()))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.DiscountId, opt => opt.MapFrom(src => src.DiscountId))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src =>
                    src.OrderDetails.Select(od => new OrderDetailResponse
                    {
                        OrderId = od.OrderId,
                        ProductId = od.ProductId,
                        Quantity = od.Quantity,
                        Price = od.Price,
                    })))
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => new UserResponse
                {
                    FullName = src.User.FullName,
                    BirthYear = src.User.BirthYear,
                    Gender = src.User.Gender,
                    Email = src.User.Email,
                    PhoneNumber = src.User.PhoneNumber,
                    AvatarPath = src.User.AvatarPath,
                    Address = src.User.Address,
                    Status = src.User.Status
                }))
                .ForMember(dest => dest.Discounts, opt => opt.MapFrom(src => new DiscountResponse
                {
                    DiscountCode = src.Discount.DiscountCode,
                    DiscountPercentage = src.Discount.DiscountPercentage,
                    StartDate = src.Discount.StartDate,
                    EndDate = src.Discount.EndDate,
                    UsageLimit = src.Discount.UsageLimit,
                    TimesUsed = src.Discount.TimesUsed,
                    Status = src.Discount.Status
                }))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src =>
                    src.OrderDetails.Select(od => od.Product).Where(product => product != null)
                    .Select(product => new ProductResponse
                    {
                        Brand = product.Brand,
                        Model = product.Model,
                        Cpu = product.Cpu,
                        Ram = product.Ram,
                        Vga = product.Vga,
                        ScreenSize = product.ScreenSize,
                        HardDisk = product.HardDisk,
                        Os = product.Os,
                        Price = product.Price,
                        Stock = product.Stock,
                        Image = product.Image,
                    }).ToList()
                ))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => new CustomerResponse
                {
                    Name = src.Customer.Name,
                    Email = src.Customer.Email,
                    PhoneNumber = src.Customer.PhoneNumber
                }))
                .ReverseMap();

            // Mapping Request
            CreateMap<OrderDetailRequest, OrderDetail>().ReverseMap();
            CreateMap<OrderRequest, Order>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ReverseMap();
            CreateMap<OrderAdminRequest, Order>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ReverseMap();
        }
    }
}
