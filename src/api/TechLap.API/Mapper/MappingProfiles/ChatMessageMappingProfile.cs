using AutoMapper;
using TechLap.API.DTOs.Responses.ChatDTOs;
using TechLap.API.Models;

namespace TechLap.API.Mapper.MappingProfiles
{
    public class ChatMessageMappingProfile : Profile
    {
        public ChatMessageMappingProfile()
        {
            CreateMap<ChatMessage, ChatMessageResponse>();

            CreateMap<SendChatMessageRequest, ChatMessage>()
                .ForMember(dest => dest.MessageContent, opt => opt.MapFrom(src => src.MessageContent))
                .ForMember(dest => dest.ReceiverId, opt => opt.MapFrom(src => src.ReceiverId));
        }
    }
}
