﻿using TechLap.API.Enums;

namespace TechLap.API.DTOs.Responses.UserDTOs
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthYear { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AvatarPath { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
    }
}
