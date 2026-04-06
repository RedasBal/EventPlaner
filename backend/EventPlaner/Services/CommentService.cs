using EventPlaner.DT0s;
using EventPlaner.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlaner.Services;

public class CommentService
{
    private readonly AppDbContext _db;

    public CommentService(AppDbContext db)
    {
        _db = db;
    }

    public bool IsParticipant(int eventId, int userId)
    {
        // Owner is always allowed to use the event chat, even if (for any reason) EventParticipants row is missing.
        var isOwner = _db.Events.AsNoTracking().Any(e => e.Id == eventId && e.OwnerId == userId);
        if (isOwner) return true;

        return _db.EventParticipants.AsNoTracking().Any(ep => ep.EventId == eventId && ep.UserId == userId);
    }

    public List<CommentResponseDto>? GetEventComments(int eventId, int userId)
    {
        var eventExists = _db.Events.AsNoTracking().Any(e => e.Id == eventId);
        if (!eventExists) return null;

        if (!IsParticipant(eventId, userId))
        {
            throw new UnauthorizedAccessException("Only event participants can view comments");
        }

        return _db.Comments
            .AsNoTracking()
            .Where(c => c.EventId == eventId)
            .Include(c => c.User)
            .OrderBy(c => c.Id)
            .Select(c => new CommentResponseDto
            {
                Id = c.Id,
                EventId = c.EventId,
                UserId = c.UserId,
                Username = c.User.Username,
                Text = c.Text
            })
            .ToList();
    }

    public CommentResponseDto CreateComment(int eventId, int userId, string text)
    {
        var ev = _db.Events.AsNoTracking().FirstOrDefault(e => e.Id == eventId);
        if (ev == null) throw new Exception("Event not found");

        var userExists = _db.Users.AsNoTracking().Any(u => u.Id == userId);
        if (!userExists) throw new Exception("User not found");

        if (!IsParticipant(eventId, userId))
        {
            throw new UnauthorizedAccessException("Only event participants can add comments");
        }

        var trimmed = (text ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new Exception("Text is required");
        }

        if (trimmed.Length > 1000)
        {
            throw new Exception("Text is too long (max 1000)");
        }

        var comment = new Comment
        {
            EventId = eventId,
            UserId = userId,
            Text = trimmed
        };

        _db.Comments.Add(comment);
        _db.SaveChanges();

        var created = _db.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .First(c => c.Id == comment.Id);

        return new CommentResponseDto
        {
            Id = created.Id,
            EventId = created.EventId,
            UserId = created.UserId,
            Username = created.User.Username,
            Text = created.Text
        };
    }

    public bool DeleteComment(int eventId, int commentId, int userId)
    {
        var ev = _db.Events.AsNoTracking().FirstOrDefault(e => e.Id == eventId);
        if (ev == null) return false;

        if (!IsParticipant(eventId, userId))
        {
            throw new UnauthorizedAccessException("Only event participants can delete comments");
        }

        var comment = _db.Comments.FirstOrDefault(c => c.Id == commentId && c.EventId == eventId);
        if (comment == null) return false;

        if (comment.UserId != userId && ev.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("Only comment author or event owner can delete this comment");
        }

        _db.Comments.Remove(comment);
        _db.SaveChanges();
        return true;
    }
}
