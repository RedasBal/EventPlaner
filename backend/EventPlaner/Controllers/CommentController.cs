using EventPlaner.DT0s;
using EventPlaner.Services;
using EventPlaner.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlaner.Controllers;

[ApiController]
[Authorize]
[Route("api/events/{eventId}/comments")]
public class CommentController : ControllerBase
{
    private readonly CommentService _comments;

    public CommentController(CommentService comments)
    {
        _comments = comments;
    }

    [HttpGet]
    public IActionResult GetComments(int eventId)
    {
        var userId = User.GetUserIdOrThrow();
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
    public IActionResult CreateComment(int eventId, [FromBody] CreateCommentDto dto)
    {
        var userId = User.GetUserIdOrThrow();
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
    public IActionResult DeleteComment(int eventId, int commentId)
    {
        var userId = User.GetUserIdOrThrow();
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
