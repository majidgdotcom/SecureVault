using System.ComponentModel.DataAnnotations;

namespace SecureVault.Application.DTOs;

public class CreateNoteDto
{
    [Required(ErrorMessage = "Content is required")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 5000 characters")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;
}