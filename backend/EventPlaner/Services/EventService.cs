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

   public List<Event> GetAllEvents()
   {
      return _db.Events
         .Include(e => e.Owner)
         .Include(e => e.Participants)
         .ToList();
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
      return ev;
   }

   public bool DeleteEvent(int id)
   {
      var ev = _db.Events.FirstOrDefault(delegate(Event ev)
      {
         return ev.Id == id;
      });
      if (ev == null)
         return false;
      _db.Events.Remove(ev);
      _db.SaveChanges();
      return true;
   }

   public Event? UpdateEvent(int id, Event ev)
   {
      var existingEvent = _db.Events.FirstOrDefault(delegate(Event e)
      {
         return e.Id == id;
      });

      if (existingEvent == null)
      {
         return null;
      }

      existingEvent.Title = ev.Title;
      existingEvent.Description = ev.Description;
      existingEvent.Location = ev.Location;
      existingEvent.OwnerId = ev.OwnerId;
      _db.SaveChanges();
      return existingEvent;
   }
}

