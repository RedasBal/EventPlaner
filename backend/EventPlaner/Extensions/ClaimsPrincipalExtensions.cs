using System.Security.Claims;

namespace EventPlaner.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserIdOrThrow(this ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId) || userId <= 0)
        {
            throw new UnauthorizedAccessException("Invalid or missing user id claim.");
        }

        return userId;
    }
}

