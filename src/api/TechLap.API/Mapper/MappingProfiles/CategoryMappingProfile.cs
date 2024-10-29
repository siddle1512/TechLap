using AutoMapper;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Responses.CategoryDTOs;
using TechLap.API.Models;

namespace TechLap.API.Mapper.MappingProfiles
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<CreateCategoryRequest, Category>().ReverseMap();
            CreateMap<CategoryResponse, Category>().ReverseMap();
        }
    }
}
