using AutoMapper;
using TechLap.API.DTOs.Requests;
using TechLap.API.Models;

namespace TechLap.API.Mapper.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserRequest>().ReverseMap();
        }
    }
}
