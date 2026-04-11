using EventPlaner.DT0s;
using EventPlaner.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlaner.Services;

public class EventService
{
   private readonly AppDbContext _db;

   public EventService(AppDbContext db)
   {
      _db = db;
   }

   public List<Event> GetAllEvents(string? q)
   {
      var query = _db.Events
         .Include(e => e.Owner)
         .Include(e => e.Participants)
         .AsQueryable();

      if (!string.IsNullOrWhiteSpace(q))
      {
         var term = $"%{q.Trim()}%";
         query = query.Where(e =>
            EF.Functions.Like(e.Title, term) ||
            EF.Functions.Like(e.Location, term));
      }

      return query.ToList();
   }

   public Event? GetAllEventById(int id)
   {
      return _db.Events
         .Include(e => e.Owner)
         .Include(e => e.Participants)
         .FirstOrDefault(delegate(Event ev)
         {
            return ev.Id == id;
         });
   }

   public Event CreateEvent(Event ev)
   {
      _db.Events.Add(ev);
      _db.SaveChanges();

      // Owner automatically becomes a participant (requirement).
      var alreadyParticipant = _db.EventParticipants.Any(ep =>
         ep.EventId == ev.Id && ep.UserId == ev.OwnerId);
      if (!alreadyParticipant)
      {
         _db.EventParticipants.Add(new EventParticipant
         {
            EventId = ev.Id,
            UserId = ev.OwnerId,
            Status = ParticipantStatus.Going
         });
         _db.SaveChanges();
      }

      // Return with navigation properties populated.
      return GetAllEventById(ev.Id) ?? ev;
   }

   public bool DeleteEvent(int id, int userId)
   {
      var ev = _db.Events
         .Include(e => e.Participants)
         .FirstOrDefault(delegate(Event ev)
         {
            return ev.Id == id;
         });
      if (ev == null)
         return false;

      if (ev.OwnerId != userId)
      {
         throw new UnauthorizedAccessException("Only owner can delete this event");
      }

      // Be explicit: remove participants first to avoid FK issues on some providers.
      if (ev.Participants.Count > 0)
      {
         _db.EventParticipants.RemoveRange(ev.Participants);
      }

      _db.Events.Remove(ev);
      _db.SaveChanges();
      return true;
   }

   public Event? UpdateEvent(int id, int userId, UpdateEventDto dto)
   {
      var existingEvent = _db.Events.FirstOrDefault(e => e.Id == id);

      if (existingEvent == null)
      {
         return null;
      }

      if (existingEvent.OwnerId != userId)
      {
         throw new UnauthorizedAccessException("Only owner can update this event");
      }

      existingEvent.Title = dto.Title;
      existingEvent.Description = dto.Description ?? string.Empty;
      existingEvent.Location = dto.Location ?? string.Empty;
      _db.SaveChanges();
      return existingEvent;
   }

   public void JoinEvent(int eventId, int userId)
   {
      var ev= _db.Events.Find(eventId);
      if(ev == null)
      {
         throw new Exception("Event not found");
      }

      var userExists = _db.Users.AsNoTracking().Any(u => u.Id == userId);
      if (!userExists)
      {
         throw new Exception("User not found");
      }

      if (userId == ev.OwnerId)
      {
         throw new Exception("You cannot join this event");
      }

      var existing = _db.EventParticipants.FirstOrDefault(delegate(EventParticipant ep)
      {
         return ep.EventId == eventId && ep.UserId == userId;
      });
      if (existing != null)
      {
         throw new Exception("Already joined");
      }

      EventParticipant ep = new EventParticipant();
      ep.EventId = eventId;
      ep.UserId = userId;
      ep.Status = ParticipantStatus.Going;
      _db.EventParticipants.Add(ep);
      _db.SaveChanges();
   }

   public void LeaveEvent(int eventId, int userId)
   {
      var participant = _db.EventParticipants.FirstOrDefault(delegate(EventParticipant ep)
      {
         return ep.EventId == eventId && ep.UserId == userId;
      });
      if (participant == null)
      {
         throw new Exception("Not participating");
      }
      _db.EventParticipants.Remove(participant);
      _db.SaveChanges();
   }

   public List<ParticipantResponseDto>? GetParticipants(int eventId)
   {
      var exists = _db.Events.AsNoTracking().Any(e => e.Id == eventId);
      if (!exists) return null;

      return _db.EventParticipants
         .AsNoTracking()
         .Where(ep => ep.EventId == eventId)
         .Include(ep => ep.User)
         .Select(ep => new ParticipantResponseDto
         {
            UserId = ep.UserId,
            Username = ep.User.Username,
            Email = ep.User.Email,
            Status = ep.Status
         })
         .ToList();
   }

   // Backwards-compatible aliases (typos happen).
   public List<ParticipantResponseDto>? GetParticipant(int eventId) => GetParticipants(eventId);
   public List<ParticipantResponseDto>? GetParicipant(int eventId) => GetParticipants(eventId);
   public List<ParticipantResponseDto>? GetParicipants(int eventId) => GetParticipants(eventId);
}
