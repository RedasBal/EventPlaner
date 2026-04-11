namespace EventPlaner.Models;

public class Comment : BaseEntity
{
    public int EventId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; }= String.Empty;
    
    public Event Event { get; set; } = null!;
    public User User { get; set; } = null!;
    
}