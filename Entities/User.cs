using System.ComponentModel.DataAnnotations;

namespace SOCRATIC_LEARNING_DOTNET.Entities;

public class User
{
    [Required]
    public string Id { get; set; } // e.g., GUID or string

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string PasswordHash { get; set; }

    public string Name { get; set; }
}
