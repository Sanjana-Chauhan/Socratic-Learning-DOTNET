using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOCRATIC_LEARNING_DOTNET.Entities
{
    public class AIResponse
    {
        [Required]
        public string Explanation { get; set; }
        public Guid conversationId { get; set; }

        [Required]
        public string FollowUpQuestion { get; set; }
        public string? Summary { get; set; } // optional
    }
}
