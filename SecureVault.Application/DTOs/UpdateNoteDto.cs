using System.ComponentModel.DataAnnotations;

namespace SecureVault.Application.DTOs;

public class UpdateNoteDto
{
    [Required(ErrorMessage = "Content is required")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 5000 characters")]
    public string Content { get; set; } = string.Empty;
}