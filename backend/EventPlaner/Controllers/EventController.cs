using EventPlaner.DT0s;
using EventPlaner.Models;
using EventPlaner.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventPlaner.Controllers;

[ApiController]
[Route("api/events")]
public class EventController : ControllerBase
{
    private readonly EventService _events;

    public EventController(EventService events)
    {
        _events = events;
    }

    [HttpGet]
    public IActionResult GetEvents()
    {
        var events = _events.GetAllEvents();
        return Ok(events);
    }

    [HttpGet("{id}")]
    public IActionResult GetEvent(int id)
    {
        var ev = _events.GetAllEventById(id);
        if (ev == null)
        {
            return NotFound("Event not found");
        }
        return Ok(ev);
    }

    [HttpPost]
    public IActionResult CreateEvent(CreateEventDto dto)
    {
        var ev = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            Location = dto.Location,
            OwnerId = dto.OwnerId,
        };
        var created = _events.CreateEvent(ev);
        return Ok(created);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteEvent(int id)
    {
        var deleted = _events.DeleteEvent(id);
        if (!deleted)
        {
            return NotFound("Event not found");
        }
        return Ok("Event deleted");
    }

    [HttpPut("{id}")]
    public IActionResult UpdateEvent(int id, CreateEventDto ev)
    {
        var updatedEvent = _events.UpdateEvent(id, new Event
        {
            Title = ev.Title,
            Description = ev.Description,
            Location = ev.Location,
            OwnerId = ev.OwnerId,
        });
        if (updatedEvent == null)
        {
            return NotFound("Event not found");
        }
        return Ok(updatedEvent);
    }
}

