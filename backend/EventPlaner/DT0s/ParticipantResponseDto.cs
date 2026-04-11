using EventPlaner.Models;

namespace EventPlaner.DT0s;

public class ParticipantResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ParticipantStatus Status { get; set; }
}

