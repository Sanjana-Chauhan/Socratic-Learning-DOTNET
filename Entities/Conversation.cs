using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOCRATIC_LEARNING_DOTNET.Entities;

public class Conversation
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
