using System.Text.Json.Serialization;

namespace EventPlaner.Models;

public class User : BaseEntity
{
    public string Username { get; set; } = String.Empty;
    public string PasswordHash { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    // Navigation
    [JsonIgnore]
    public List<Event> OwnedEvents { get; set; } = new();
    public List<EventParticipant> Participations { get; set; } = new();
}