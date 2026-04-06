using EventPlaner.Models;
using Microsoft.EntityFrameworkCore;


namespace EventPlaner.Services;

public class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public User? Login(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = _db.Users.FirstOrDefault(u => u.Email.ToLower() == normalizedEmail);
        if (user == null) return null;

        var stored = user.PasswordHash ?? string.Empty;

        var ok = false;
        try
        {
            ok = BCrypt.Net.BCrypt.Verify(password, stored);
        }
        catch
        {
            ok = false;
        }

        // Legacy support: if old users were saved with plain-text password in PasswordHash column,
        // allow login once and upgrade to BCrypt hash.
        if (!ok && stored == password)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            _db.SaveChanges();
            ok = true;
        }

        return ok ? user : null;
    }

    public User? Register(string username, string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var normalizedUsername = username.Trim();

        var exists = _db.Users.Any(u => u.Email.ToLower() == normalizedEmail || u.Username == normalizedUsername);
        if (exists) return null;

        var newUser = new User
        {
            Username = normalizedUsername,
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _db.Users.Add(newUser);
        _db.SaveChanges();
        return newUser;
    }
}
