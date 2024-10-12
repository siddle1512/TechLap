using TechLap.API.Enums;

namespace TechLap.API.DTOs.Requests
{
    public record UserRequest(
        string FullName,
        DateTime BirthYear,
        Gender Gender,
        string Email,
        string HashedPassword,
        string AvatarPath,
        string Address,
        UserStatus Status
    );
}
