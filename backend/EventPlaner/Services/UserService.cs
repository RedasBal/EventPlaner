using EventPlaner.Models;

namespace EventPlaner.Services;

public class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public User? Login(string email, string passwordHash)
    {
        foreach (var user in _db.Users)
        {
            if (user.Email == email && user.PasswordHash == passwordHash)
            {
                return user;
            }
        }

        return null;
    }

    public User? Register(string username, string email, string passwordHash)
    {
        foreach (var user in _db.Users)
        {
            if (user.Email == email || user.Username == username)
            {
                return null;
            }
        }

        var newUser = new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash
        };

        _db.Users.Add(newUser);
        _db.SaveChanges();
        return newUser;
    }
}

