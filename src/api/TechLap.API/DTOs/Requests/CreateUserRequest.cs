using TechLap.API.Enums;

namespace TechLap.API.DTOs.Requests
{
    public record CreateUserRequest(
        string FullName,
        DateTime BirthYear,
        Gender Gender,
        string Email,
        string PhoneNumber,
        string HashedPassword,
        string AvatarPath,
        string Address,
        UserStatus Status = UserStatus.Inactive
    );
}
