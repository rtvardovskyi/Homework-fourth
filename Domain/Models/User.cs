using Domain.Enums;

namespace Domain.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Username { get; set; } = string.Empty;
    
    public UserRoles Role { get; set; }
    
    public byte[] PasswordHash { get; set; }
    
    public byte[] PasswordSalt { get; set; }
}