namespace EventPlaner.Models;

public class EventParticipant : BaseEntity
{
    public int EventId { get; set; }
    public int UserId { get; set; }

    public ParticipantStatus Status { get; set; }
    public Event Event { get; set; } = null!;
    public User User { get; set; } = null!;
    
}