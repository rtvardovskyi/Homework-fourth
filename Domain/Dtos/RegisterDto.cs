using Domain.Enums;

namespace Domain.Dtos;

public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    
    public UserRoles Role { get; set; }

    public string Password { get; set; } = string.Empty;
}