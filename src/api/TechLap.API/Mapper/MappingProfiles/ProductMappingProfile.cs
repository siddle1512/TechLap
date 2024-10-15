using AutoMapper;
using TechLap.API.DTOs.Requests;
using TechLap.API.Models;

namespace TechLap.API.Mapper.MappingProfiles
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<SearchProductsRequest, Product>();
        }
    }
}
