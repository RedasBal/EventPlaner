namespace EventPlaner.Models;

public class Event : BaseEntity
{
    public string Title { get; set; } =  String.Empty;
    public string Description{get; set;} = String.Empty;
    public string Location { get; set; } = String.Empty;
    public int OwnerId { get; set; }
    // Navigation
    public User Owner { get; set; } = null!;
    public List<EventParticipant> Participants { get; set; } = new();
    //public List<Comment> Comments { get; set; } = new();
}