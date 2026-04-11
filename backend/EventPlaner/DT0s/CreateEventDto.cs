using System.ComponentModel.DataAnnotations;

namespace EventPlaner.DT0s;

public class CreateEventDto
{
    [Required]
    [StringLength(120, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(200)]
    public string Location { get; set; } = string.Empty;
}

