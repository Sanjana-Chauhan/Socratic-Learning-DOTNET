using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOCRATIC_LEARNING_DOTNET.Entities
{
    public class AIResponse
    {
        
        public Guid conversationId { get; set; }
        public string Response { get; set; }
    }
}
