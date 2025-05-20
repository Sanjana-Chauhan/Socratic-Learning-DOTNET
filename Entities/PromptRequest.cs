namespace SOCRATIC_LEARNING_DOTNET.Entities;

public class PromptRequest
{
    public string UserId { get; set; }
    public string Prompt { get; set; }
    public Guid? ConversationId { get; set; } = null; // Optional if starting a new conversation
}
