using EventPlaner.DT0s;
using EventPlaner.Models;
using EventPlaner.Services;
using EventPlaner.Extensions;
using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
    public IActionResult GetEvents([FromQuery] string? q)
    {
        var events = _events.GetAllEvents(q);
        return Ok(events);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
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
    [Authorize]
    public IActionResult CreateEvent([FromBody] CreateEventDto dto)
    {
        var ownerId = User.GetUserIdOrThrow();
        var ev = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            Location = dto.Location,
            OwnerId = ownerId,
        };
        var created = _events.CreateEvent(ev);
        return Ok(created);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult DeleteEvent(int id)
    {
        var userId = User.GetUserIdOrThrow();
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
    [Authorize]
    public IActionResult UpdateEvent(int id, [FromBody] UpdateEventDto ev)
    {
        var userId = User.GetUserIdOrThrow();
        try
        {
            var updatedEvent = _events.UpdateEvent(id, userId, ev);
            if (updatedEvent == null)
            {
                return NotFound("Event not found");
            }
            return Ok(updatedEvent);
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(403, e.Message);
        }
    }

    [HttpPost("{id}/join")]
    [Authorize]
    public IActionResult JoinEvent(int id)
    {
        var userId = User.GetUserIdOrThrow();
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
    [Authorize]
    public IActionResult LeaveEvent(int id)
    {
        var userId = User.GetUserIdOrThrow();
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
    [AllowAnonymous]
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
