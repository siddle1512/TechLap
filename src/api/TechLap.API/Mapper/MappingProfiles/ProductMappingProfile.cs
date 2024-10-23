using AutoMapper;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Responses.ProductDTOs;
using TechLap.API.Models;

namespace TechLap.API.Mapper.MappingProfiles
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<SearchProductsRequest, Product>().ReverseMap();
            CreateMap<ProductResponse, Product>().ReverseMap();
            CreateMap<Product, ProductRequest>().ReverseMap();
        }
    }
}
