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
    public IActionResult DeleteEvent(int id, [FromQuery] int userId)
    {
        try
        {
            var deleted = _events.DeleteEvent(id, userId);
            if (!deleted)
            {
                return NotFound("Event not found");
            }
            return Ok("Event deleted");
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(403, e.Message);
        }
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

    [HttpPost("{id}/join")]
    public IActionResult JoinEvent(int id, [FromQuery] int userId)
    {
        try
        {
            _events.JoinEvent(id, userId);
            return Ok("Event joined");
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{id}/leave")]
    public IActionResult LeaveEvent(int id, [FromQuery] int userId)
    {
        try
        {
            _events.LeaveEvent(id, userId);
            return Ok("Event leaved");
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}/participants")]
    public IActionResult GetParticipants(int id)
    {
        var participants = _events.GetParticipants(id);
        if (participants == null)
        {
            return NotFound("Event not found");
        }
        return Ok(participants);
    }
}
