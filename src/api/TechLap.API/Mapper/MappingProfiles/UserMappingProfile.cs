using AutoMapper;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Responses.UserDTOs;
using TechLap.API.Models;

namespace TechLap.API.Mapper.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, CreateUserRequest>().ReverseMap();
            CreateMap<User, UserResponse>().ReverseMap();
            CreateMap<User, UpdateUserRequest>().ReverseMap();
        }
    }
}
