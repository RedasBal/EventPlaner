using System.ComponentModel.DataAnnotations;

namespace EventPlaner.DT0s;

public class CreateCommentDto
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}

