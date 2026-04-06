using EventPlaner.DT0s;
using EventPlaner.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventPlaner.Controllers;

[ApiController]
[Route("api/events/{eventId}/comments")]
public class CommentController : ControllerBase
{
    private readonly CommentService _comments;

    public CommentController(CommentService comments)
    {
        _comments = comments;
    }

    // Chat/comments are visible only for participants.
    // Frontend passes current userId as query param (simple auth model used in this project).
    [HttpGet]
    public IActionResult GetComments(int eventId, [FromQuery] int userId)
    {
        try
        {
            var comments = _comments.GetEventComments(eventId, userId);
            if (comments == null) return NotFound("Event not found");
            return Ok(comments);
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(403, e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public IActionResult CreateComment(int eventId, [FromQuery] int userId, [FromBody] CreateCommentDto dto)
    {
        try
        {
            var created = _comments.CreateComment(eventId, userId, dto?.Text ?? string.Empty);
            return Ok(created);
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(403, e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{commentId}")]
    public IActionResult DeleteComment(int eventId, int commentId, [FromQuery] int userId)
    {
        try
        {
            var deleted = _comments.DeleteComment(eventId, commentId, userId);
            if (!deleted) return NotFound("Comment not found");
            return Ok("Comment deleted");
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(403, e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}

