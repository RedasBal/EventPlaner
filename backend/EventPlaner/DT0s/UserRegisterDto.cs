using System.ComponentModel.DataAnnotations;

namespace EventPlaner.DT0s;

public class UserRegisterDto
{
    [Required]
    [StringLength(40, MinimumLength = 2)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // Preferred field name from frontend
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    // Backwards-compatible: some clients still send "passwordHash"
    public string PasswordHash { get; set; } = string.Empty;
}

