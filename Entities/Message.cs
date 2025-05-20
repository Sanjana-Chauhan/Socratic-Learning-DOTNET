using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOCRATIC_LEARNING_DOTNET.Entities;

public class Message
{
    [Key]
    public Guid id { get; set; }

    public Guid conversationId { get; set; }

    [Required]
    public string role { get; set; }

    [Required]
    public string content { get; set; }

    [Required]
    public DateTime timestamp { get; set; }
}
