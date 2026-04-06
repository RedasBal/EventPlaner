namespace EventPlaner.DT0s;

public class UserRegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Preferred field name from frontend
    public string Password { get; set; } = string.Empty;

    // Backwards-compatible: some clients still send "passwordHash"
    public string PasswordHash { get; set; } = string.Empty;
}

