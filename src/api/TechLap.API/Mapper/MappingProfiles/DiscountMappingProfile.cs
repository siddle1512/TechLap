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
            CreateMap<AddAdminDiscountRequest, Discount>();
            CreateMap<UpdateAdminDiscountRequest, Discount>();
            CreateMap<DeleteAdminDiscountRequest, Discount>();
            CreateMap<GetAdminDiscountRespones, Discount>();
            CreateMap<ApplyUserDiscountRequest, Discount>();
        }
    }
}
