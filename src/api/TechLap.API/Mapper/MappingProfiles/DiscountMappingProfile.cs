using AutoMapper;
using TechLap.API.DTOs.Requests.DiscountRequests;
using TechLap.API.DTOs.Responses.DiscountRespones;
using TechLap.API.Models;

namespace TechLap.API.Mapper.MappingProfiles
{
    public class DiscountMappingProfile : Profile
    {
        public DiscountMappingProfile()
        {
            CreateMap<AddAdminDiscountRequest, Discount>().ReverseMap();
            CreateMap<UpdateAdminDiscountRequest, Discount>().ReverseMap();
            CreateMap<DeleteAdminDiscountRequest, Discount>().ReverseMap();
            CreateMap<GetAdminDiscountRespones, Discount>().ReverseMap();
            CreateMap<ApplyUserDiscountRequest, Discount>().ReverseMap();
        }
    }
}
